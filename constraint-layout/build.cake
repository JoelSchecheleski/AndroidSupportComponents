#tool nuget:?package=XamarinComponent

#addin nuget:?package=Cake.Xamarin
#addin nuget:?package=Cake.Xamarin.Build

var TARGET = Argument ("target", Argument ("t", "Default"));


var NUGET_VERSION = "1.0.2.2";
var AAR_VERSION = "1.0.2";

var CONSTRAINT_LAYOUT_URL = string.Format ("https://dl.google.com/dl/android/maven2/com/android/support/constraint/constraint-layout/{0}/constraint-layout-{0}.aar", AAR_VERSION);
var CONSTRAINT_LAYOUT_SOLVER_URL = string.Format ("https://dl.google.com/dl/android/maven2/com/android/support/constraint/constraint-layout-solver/{0}/constraint-layout-solver-{0}.jar", AAR_VERSION);


var buildSpec = new BuildSpec {
	Libs = new [] {
		new DefaultSolutionBuilder {
			SolutionPath = "./ConstraintLayout.sln",
			BuildsOn = BuildPlatforms.Windows | BuildPlatforms.Mac,
			OutputFiles = new [] {
				new OutputFileCopy { FromFile = "./source/bin/Release/Xamarin.Android.Support.Constraint.Layout.dll", ToDirectory = "./output/" },
				new OutputFileCopy { FromFile = "../constraint-layout-solver/source/bin/Release/Xamarin.Android.Support.Constraint.Layout.Solver.dll", ToDirectory = "./output/" },
			}
		}
	},

	Samples = new [] {
		new DefaultSolutionBuilder { SolutionPath = "./samples/ConstraintLayoutSample.sln", BuildsOn = BuildPlatforms.Windows | BuildPlatforms.Mac },
	},

	NuGets = new [] {
		new NuGetInfo { NuSpec = "./nuget/Xamarin.Android.Support.Constraint.Layout.nuspec", Version = NUGET_VERSION, RequireLicenseAcceptance = true },
		new NuGetInfo { NuSpec = "../constraint-layout-solver/nuget/Xamarin.Android.Support.Constraint.Layout.Solver.nuspec", Version = NUGET_VERSION, RequireLicenseAcceptance = true },
	},
};


Task ("externals")
	.WithCriteria (() => !FileExists ("../externals/constraintlayout/classes.jar")).Does (() =>
{
	var path = "../externals/";

	EnsureDirectoryExists (path);

	if (!FileExists (path + "constraint-layout.aar"))
		DownloadFile (CONSTRAINT_LAYOUT_URL, path + "constraint-layout.aar");
	if (!FileExists (path + "constraint-layout-solver.jar"))
		DownloadFile (CONSTRAINT_LAYOUT_SOLVER_URL, path + "constraint-layout-solver.jar");
});

Task ("clean").IsDependentOn ("clean-base").Does (() => 
{
	if (DirectoryExists ("../externals/constraintlayout"))
		DeleteDirectory ("../externals/constraintlayout", true);
	if (FileExists ("../externals/constraintlayout.zip"))
		DeleteFile ("../externals/constraintlayout.zip");

	if (DirectoryExists ("../externals/solver"))
		DeleteDirectory ("../externals/solver", true);
	if (FileExists ("../externals/solver.zip"))
		DeleteFile ("../externals/solver.zip");

	if (DirectoryExists ("./output"))
		DeleteDirectory ("./output", true);
});

SetupXamarinBuildTasks (buildSpec, Tasks, Task);

RunTarget (TARGET);
