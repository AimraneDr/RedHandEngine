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
using System.Windows;
using System.Windows.Input;
using Editor.GameDev;

namespace Editor.GameProjectFiles
{
    [DataContract(Name = "GameProject")]
    internal class Project : ViewModelBase
    {
        #region Properties
        public static readonly string ProjectPrivateFolderName = @".Sky\";
        public static readonly string ProjectGameScriptsFolderName = @"GameScripts\";
        public static string Extension = ".sky";
        [DataMember]
        public string Name { get; private set; }
        [DataMember]
        public string Path { get; private set; }
        public string Solution => $"{Path}{Name}.sln";
        public string FullPath => $@"{Path}{Name}{Extension}";
        [DataMember(Name = "Scenes")]
        private ObservableCollection<Scene> scenes = new ObservableCollection<Scene>();
        public static ReadOnlyObservableCollection<Scene> ScenesCollection { get; private set; }

        public static Project Current { get; set; } = Application.Current.MainWindow.DataContext as Project;

        private Scene active_scene;
        public Scene ActiveScene
        {
            get=> active_scene;
            set
            {
                if(active_scene != value)
                {
                    active_scene = value;
                    OnPropertyChanged(nameof(ActiveScene));
                }
            }
        }

        public static UndoRedo UndoRedo { get; private set; } = new UndoRedo();

        static public ICommand UndoCommand { get; private set; }
        static public ICommand RedoCommand { get; private set; }
        static public ICommand AddSceneCommand { get; private set; }
        static public ICommand RemoveSceneCommand { get; private set; }
        static public ICommand SaveCommand { get; private set; }

        #endregion

        public Project(string name,string path)
        {
            Name = name;
            Path = path;

            OnDeserialized(new StreamingContext());
        }
        static Project() { }

        [OnDeserialized]
        private void OnDeserialized(StreamingContext context)
        {
            if (scenes != null)
            {
                ScenesCollection = new ReadOnlyObservableCollection<Scene>(scenes);
                OnPropertyChanged(nameof(ScenesCollection));
            }
            ActiveScene = ScenesCollection.FirstOrDefault(x => x.IsActive);

            AddSceneCommand = new RelayCommand<object>(x =>
            {
                AddScene("New Scene " + $"{ScenesCollection.Count}");
                var newScene = ScenesCollection.Last();
                var sceneIndex = ScenesCollection.Count - 1;
                UndoRedo.Add(new UndoRedoAction(
                    () => RemoveScene(newScene),
                    () => scenes.Insert(sceneIndex, newScene),
                    $"Add {newScene.Name}"
                    ));
            });
            RemoveSceneCommand = new RelayCommand<Scene>(x =>
            {
                var sceneIndex = scenes.IndexOf(x);
                RemoveScene(x);
                UndoRedo.Add(new UndoRedoAction(
                      () => scenes.Insert(sceneIndex, x),
                      () => RemoveScene(x),
                      $"Remove {x.Name}"
                  ));
            }, x => !x.IsActive);

            UndoCommand = new RelayCommand<object>(x => UndoRedo.Undo());
            RedoCommand = new RelayCommand<object>(x => UndoRedo.Redo());
            SaveCommand = new RelayCommand<object>(x => Save(this));
        }

        /*
         * ---------------------------------------------------------------------------
         * ---------------------------------------------------------------------------
         *      START OF : PROJECT RELATED FUNCTIONS
         * ---------------------------------------------------------------------------
         * ---------------------------------------------------------------------------
         */
        public static Project Load(string file)
        {
            Debug.Assert(File.Exists(file));
            return Serializer.FromFile<Project>(file);
        }
        public static void Save(Project file)
        {
            Serializer.ToFile(file, file.FullPath);
            Logger.Log(MessageType.Info, $"Project Saved to : '{file.FullPath}' .");
        }
        public void Unload()
        {
            VisualStudio.CloseVisualStudio();
            UndoRedo.Reset();
        }
        /*
         * ---------------------------------------------------------------------------
         * ---------------------------------------------------------------------------
         *      END OF : PROJECT RELATED FUNCTIONS
         * ---------------------------------------------------------------------------
         * ---------------------------------------------------------------------------
         */
        /*
         * ---------------------------------------------------------------------------
         * ---------------------------------------------------------------------------
         *      START OF : SCENE RELATED FUNCTIONS
         * ---------------------------------------------------------------------------
         * ---------------------------------------------------------------------------
         */
       
        private void AddScene(string scene_name)
        {
            Debug.Assert(!string.IsNullOrEmpty(scene_name.Trim()));
            scenes.Add(new Scene(this, scene_name));
        }
        private void RemoveScene(Scene scene)
        {
            Debug.Assert(scenes.Contains(scene));
            scenes.Remove(scene);
        }
        /*
         * ---------------------------------------------------------------------------
         * ---------------------------------------------------------------------------
         *      END OF : SCENE RELATED FUNCTIONS
         * ---------------------------------------------------------------------------
         * ---------------------------------------------------------------------------
         */

    }
}
