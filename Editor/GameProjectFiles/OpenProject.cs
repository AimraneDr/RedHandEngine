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

namespace Editor.GameProjectFiles
{
    internal class OpenProject
    {
        private static readonly string
            _ApplicationDataPath = @$"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\Editor\",
            _ProjectsDataPath;
           
        private static readonly ObservableCollection<ProjectData> _projectsdata = new ObservableCollection<ProjectData>();
        public static ReadOnlyObservableCollection<ProjectData> Projects { get; }

        static OpenProject()
        {
            try
            {
                if (!Directory.Exists(_ApplicationDataPath)) Directory.CreateDirectory(_ApplicationDataPath);
                _ProjectsDataPath = $@"{_ApplicationDataPath}projectsdata.xml";
                Projects = new ReadOnlyObservableCollection<ProjectData>(_projectsdata);
                ReadProjectData();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                Logger.Log(MessageType.Error, $"Faild to Read Project's Data.");
                throw;
            }
        }

        public static Project Open(ProjectData data)
        {
            ReadProjectData();
            var _project = _projectsdata.FirstOrDefault(x => x.ProjectFullPath == data.ProjectFullPath);
            if (_project != null)
            {
                _project.LastOpen = DateTime.UtcNow;
            }
            else
            {
                _project = data;
                _project.LastOpen = DateTime.UtcNow;
                _projectsdata.Add(_project);
            }
            WriteProjectData();
            return Project.Load(_project.ProjectFullPath);
        }
        private static void ReadProjectData()
        {
            if (File.Exists(_ProjectsDataPath))
            {
                var projects_list = Serializer.FromFile<ProjectsDataList>(_ProjectsDataPath).projects.OrderByDescending(x => x.LastOpen);
                _projectsdata.Clear();
                foreach (var project in projects_list)
                {
                    if (File.Exists(project.ProjectFullPath))
                    {
                        project.Icon = File.ReadAllBytes($@"{project.ProjectPath}\{Project.ProjectPrivateFolderName}icon.png");
                        project.ScreenShot = File.ReadAllBytes($@"{project.ProjectPath}\{Project.ProjectPrivateFolderName}scrennshot.png");
                        _projectsdata.Add(project);
                    }
                }
            }
        }
        private static void WriteProjectData()
        {
            var projects_list = _projectsdata.OrderBy(x => x.LastOpen).ToList();
            Serializer.ToFile(new ProjectsDataList() { projects = projects_list }, _ProjectsDataPath);
        }
    }

    [DataContract]
    public class ProjectsDataList
    {
        [DataMember]
        public List<ProjectData> projects { get; set; }
    }
    [DataContract]
    public class ProjectData
    {
        [DataMember]
        public string ProjectName { get; set; }
        [DataMember]
        public string ProjectPath { get; set; }
        public string ProjectFullPath { get => $"{ProjectPath}{ProjectName}{Project.Extension}"; }
        [DataMember]
        public DateTime LastOpen { get; set; }
        public byte[] Icon { get; set; }
        public byte[] ScreenShot { get; set; }
    }
}
