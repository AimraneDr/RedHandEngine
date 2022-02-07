using Editor.Utilities;
using Editor.Utilities.Logs;
using Editor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Editor.GameProjectFiles
{
    public class NewProject : ViewModelBase
    {
        //TODO: use the path from the instalition
        private readonly string _template_directory = @"..\..\Editor\ProjectTemplates";
        private string _name = "NewProject";
        public string ProjectName
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    ValidateProjectPath();
                    OnPropertyChanged(nameof(ProjectName));
                }
            }
        }

        private string _path = $@"{Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)}\EngineProjects";
        public string ProjectPath
        {
            get => _path;
            set
            {
                if (_path != value)
                {
                    _path = value;
                    ValidateProjectPath();
                    OnPropertyChanged(nameof(ProjectPath));
                }
            }
        }

        private bool _pathIsValid;
        public bool PathIsValid
        {
            get=> _pathIsValid;
            set
            {
                if (_pathIsValid != value)
                {
                    _pathIsValid = value;
                    OnPropertyChanged(nameof(PathIsValid));
                }
            }
        }


        private string _ErrorMsg;
        public string ErrorMsg
        {
            get => _ErrorMsg;
            set
            {
                if (_ErrorMsg != value)
                {
                    _ErrorMsg = value;
                    OnPropertyChanged(nameof(ErrorMsg));
                }
            }
        }


        private ObservableCollection<ProjectTemplate> _templates = new ObservableCollection<ProjectTemplate>();
        public ReadOnlyObservableCollection<ProjectTemplate> ProjectTemplates { get; }

        public NewProject()
        {
            ProjectTemplates = new ReadOnlyObservableCollection<ProjectTemplate>(_templates);
            try
            {
                var templates = Directory.GetFiles(_template_directory, "template.xml", SearchOption.AllDirectories);
                Debug.Assert(templates.Any());
                foreach (var file in templates)
                {
                    var template = Serializer.FromFile<ProjectTemplate>(file);
                    template.IconFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), "icon.png"));
                    template.Icon = File.ReadAllBytes(template.IconFilePath);
                    template.ScreenShotFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), "scrennshot.png"));
                    template.ScrennShot = File.ReadAllBytes(template.ScreenShotFilePath);
                    template.ProjectFilePath = Path.GetFullPath(Path.Combine(Path.GetDirectoryName(file), template.ProjectFile));
                    template.TemplatePath = Path.GetDirectoryName(file);
                    _templates.Add(template);
                }
                ValidateProjectPath();

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Logger.Log(MessageType.Error, $"Faild to Create {ProjectName}.");
                throw;
            }
        }

        bool ValidateProjectPath()
        {

            string path = ProjectPath;
            if (!Path.EndsInDirectorySeparator(path)) path += @"\";
            path += @$"{ProjectName}\";

            PathIsValid = false;
            if (string.IsNullOrEmpty(ProjectName) || string.IsNullOrWhiteSpace(ProjectName.Trim()))
            {
                ErrorMsg = "Need to Type a Project Name .";
            }
            else if (ProjectName.IndexOfAny(Path.GetInvalidFileNameChars()) != -1)
            {
                ErrorMsg = "Invalid Character(s) used in Project Name.";
            }
            else if (string.IsNullOrEmpty(ProjectPath) || string.IsNullOrWhiteSpace(ProjectPath.Trim()))
            {
                ErrorMsg = "Need to Choose a Project Path .";
            }
            else if (ProjectPath.IndexOfAny(Path.GetInvalidPathChars()) != -1)
            {
                ErrorMsg = "Invalid Character(s) used in Project Path.";
            }
            else if (Directory.Exists(path) && Directory.EnumerateFileSystemEntries(path).Any())
            {
                ErrorMsg = "Selected Project Path Already Exists and Is Not Empty";
            }
            else
            {
                ErrorMsg = string.Empty;
                PathIsValid = true;
            }
            return PathIsValid;
        }

        public string CreateProject(ProjectTemplate template)
        {
            if(!ValidateProjectPath())return string.Empty;

            if (!Path.EndsInDirectorySeparator(ProjectPath)) ProjectPath += @"\";
            var path = @$"{ProjectPath}{ProjectName}\";
            try
            {
                if(!Directory.Exists(path)) Directory.CreateDirectory(path);
                foreach(var folder in template.FolderNames)
                {
                    Directory.CreateDirectory(Path.GetFullPath(Path.Combine(Path.GetDirectoryName(path),folder)));
                }
                var dirInfo = new DirectoryInfo(path + Project.ProjectPrivateFolderName);
                dirInfo.Attributes |= FileAttributes.Hidden;
                File.Copy(template.IconFilePath, Path.GetFullPath(Path.Combine(dirInfo.FullName, "icon.png")));
                File.Copy(template.ScreenShotFilePath, Path.GetFullPath(Path.Combine(dirInfo.FullName, "scrennshot.png")));

                var project_file = File.ReadAllText(template.ProjectFilePath);
                project_file = string.Format(project_file, ProjectName, ProjectPath);
                var project_path = Path.GetFullPath(Path.Combine(path, $"{ProjectName}{Project.Extension}"));
                File.WriteAllText(project_path, project_file);

                CreateProjectSolution(template,path);

                return path;
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Logger.Log(MessageType.Error, $"Faild to Read Project Template .");
                throw;
            }
        }

        private void CreateProjectSolution(ProjectTemplate template, string project_path)
        {
            Debug.Assert(File.Exists(Path.Combine(template.TemplatePath, "SolutionTemplate")) && File.Exists(Path.Combine(template.TemplatePath, "ProjectTemplate")));

            var engineAPIPath = Path.Combine(MainWindow.EnginePath, @"Engine\EngineAPI\");
            Debug.Assert(Directory.Exists(engineAPIPath));

            var _0 = ProjectName;
            var _1 = "{" + Guid.NewGuid().ToString().ToUpper() + "}";
            var _2 = engineAPIPath;
            var _3 = MainWindow.EnginePath;

            var solution = File.ReadAllText(Path.Combine(template.TemplatePath, "SolutionTemplate"));
            solution = string.Format(solution, _0, _1, "{" + Guid.NewGuid().ToString().ToUpper() + "}");

            var project = File.ReadAllText(Path.Combine(template.TemplatePath, "ProjectTemplate"));
            project = string.Format(project, _0, _1, _2, _3);

            File.WriteAllText(Path.GetFullPath(Path.Combine(project_path, $"{_0}.sln")),solution);
            File.WriteAllText(Path.GetFullPath(Path.Combine(project_path, @$"Scripts\{_0}.vsproj")), project);
        }
    }

    [DataContract]
    public class ProjectTemplate
    {
        [DataMember]
        public string ProjectType { get; set; }
        [DataMember]
        public string ProjectFile { get; set; }
        [DataMember]
        public List<string> FolderNames { get; set; }

        public byte[] Icon { get; set; }
        public byte[] ScrennShot { get; set; }
        public string IconFilePath { get; set; }
        public string ScreenShotFilePath { get; set; }
        public string ProjectFilePath { get; set; }
        public string TemplatePath { get; set; }
    }
}
