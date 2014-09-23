<?xml version="1.0" encoding="utf-8"?>
<Project xmlns="http://schemas.microsoft.com/developer/msbuild/2003" ToolsVersion="4.0"  DefaultTargets="FxCop">
  <!--Refer to the MSBuildCommunityTasks to perform XmlRead. This is used to read and process the
  FxCop Analysis XML and check if there are any errors or warnings.-->
  <PropertyGroup>
    <ProjectGuid>{52B0EA3B-37E3-428F-B57B-2C884E08150F}</ProjectGuid>
  </PropertyGroup>
  <UsingTask AssemblyFile="E:\TestCCnet\ThirdParty\MSBuildTasks.1.4.0.65\tools\MSBuild.Community.Tasks.dll" TaskName="MSBuild.Community.Tasks.XmlRead" />
  <!--<UsingTask TaskName="ReportGenerator" AssemblyFile="ReportGenerator.exe" />-->
  <!-- ItemGroup helps avoid duplication so let us define one for the buildartifacts folder which will be used in Clean, Init Targets.-->
  <ItemGroup>
    <BuildArtifacts Include=".\buildartifacts\" />
    <SolutionFile Include=".\TestCCnet.sln" />
  </ItemGroup>
  <!--FxCop-->
  <ItemGroup>
    <FxCop Include="E:\TestCCnet\ThirdParty\FxCop\FxCopCmd.exe" />
    <AssembliesToAnalyze Include="E:\TestCCnet\FxCopTest.FxCop" />
    <AnalysisReport Include="E:\TestCCnet\buildartifacts\FxCopAnalysis.xml" />
  </ItemGroup>

  <ItemGroup>
    <None Include="MSBuild_MasterConfigFile.BUILD" />
  </ItemGroup>

  <!--Clean-->
  <Target Name="Clean">
    <RemoveDir Directories="@(BuildArtifacts)" />
  </Target>
  <!--Init-->
  <Target Name="Init" DependsOnTargets="Clean">
    <MakeDir Directories="@(BuildArtifacts)" />
  </Target>
  <!--Compile-->
  <Target Name="Compile" DependsOnTargets="Init">
    <!-- We need to specify the full path for OutDir in MSBuild Properties.
    Hence we need to use %(BuildArtifacts.FullPath) which return the full path.-->
    <MSBuild Projects="@(SolutionFile)" Targets="Rebuild" Properties="OutDir=%(BuildArtifacts.FullPath);Configuration=Release">
    </MSBuild>
  </Target>
  <!--FxCop-->
  <Target Name="FxCop" DependsOnTargets="Compile">
    <!--The command below runs FxCop analysis on the assemblies specified using the /file: and
    uses the referenced assemblies using the /reference: and generates the report specified
    using the /out: parameter. -->
    <Exec Command="@(FxCop) /project:@(AssembliesToAnalyze) /out:@(AnalysisReport)" />

    <!--<PropertyGroup>
      <FxCopCriticalErrors>0</FxCopCriticalErrors>
      <FxCopErrors>0</FxCopErrors>
      <FxCopCriticalWarnings>0</FxCopCriticalWarnings>
    </PropertyGroup>
    <XmlRead ContinueOnError="True" XmlFileName="@(AnalysisReport)" XPath="string(count(//Issue[@Level='CriticalError']))">
      <Output TaskParameter="Value" PropertyName="FxCopCriticalErrors" />
    </XmlRead>
    <XmlRead ContinueOnError="True" XmlFileName="@(AnalysisReport)" XPath="string(count(//Issue[@Level='Error']))">
      <Output TaskParameter="Value" PropertyName="FxCopErrors" />
    </XmlRead>
    <XmlRead ContinueOnError="True" XmlFileName="@(AnalysisReport)" XPath="string(count(//Issue[@Level='CriticalWarning']))">
      <Output TaskParameter="Value" PropertyName="FxCopCriticalWarnings" />
    </XmlRead>
    <Error Text="FxCop encountered $(Count) material rule violations" Condition="$(FxCopCriticalErrors) &gt; 50 or $(FxCopErrors) &gt; 50 or $(FxCopCriticalWarnings) &gt; 50" />-->
  </Target>

</Project>