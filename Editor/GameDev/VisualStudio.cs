using Editor.Utilities.Logs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace Editor.GameDev
{
    static class VisualStudio
    {
        private static EnvDTE80.DTE2 _vsInstamce = null;
        private static readonly string _progID = "VisualStudio.DTE.17.0";

        [DllImport("ole32.dll")]
        private static extern int CreateBindCtx(uint reserved, out IBindCtx ppbc);

        [DllImport("ole32.dll")]
        private static extern int GetRunningObjectTable(uint reserved, out IRunningObjectTable pprot);
        public static void OpenVisualStudion(string solutionPath)
        {
            IRunningObjectTable rot = null;
            IEnumMoniker monikerTb = null;
            IBindCtx bindCtx = null;
            try
            {
                if (_vsInstamce == null)
                {
                    //find and open visual studio
                    var hResult = GetRunningObjectTable(0, out rot);
                    if (hResult < 0 || rot == null) throw new COMException($"GetRunningObjectTable() returned HRESULT: {hResult:x8}");

                    rot.EnumRunning(out monikerTb);
                    monikerTb.Reset();
                    hResult = CreateBindCtx(0, out bindCtx);
                    if (hResult < 0 || bindCtx == null) throw new COMException($"CreateBindCtx() returned HRESULT: {hResult:x8}");

                    IMoniker[] currentMoniker = new IMoniker[1];
                    while(monikerTb.Next(1,currentMoniker,IntPtr.Zero) == 0)
                    {
                        string name = string.Empty;
                        currentMoniker[0].GetDisplayName(bindCtx, null, out name);
                        if (name.Contains(_progID))
                        {
                            hResult = rot.GetObject(currentMoniker[0], out object obj);
                            if (hResult < 0 || obj == null) throw new COMException($"Running object table's GetObject() returned HRESULT: {hResult:x8}");
                            EnvDTE80.DTE2 dte = obj as EnvDTE80.DTE2;
                            var solution_name = dte.Solution.FileName;
                            if(solution_name == solutionPath)
                            {
                                _vsInstamce = dte;
                                break;
                            }
                        }
                    }
                    //if we couldn't find visual studio instanse we'll create a new one
                    if (_vsInstamce == null)
                    {
                        Type vsType = Type.GetTypeFromProgID(_progID, true);
                        _vsInstamce = Activator.CreateInstance(vsType) as EnvDTE80.DTE2;
                    }
                }
            }catch(Exception ex)
            {
                Debug.WriteLine(ex.Message); ;
                Logger.Log(MessageType.Error, "Faild to open VisualStudio : \n" + ex.Message);
            }
            finally
            {
                if(monikerTb != null) Marshal.ReleaseComObject(monikerTb);
                if(rot!=null) Marshal.ReleaseComObject(rot);
                if(bindCtx!=null) Marshal.ReleaseComObject(bindCtx);
            }
        }

        public static void CloseVisualStudio()
        {
            if (_vsInstamce?.Solution.IsOpen == true)
            {
                _vsInstamce.ExecuteCommand("File.SaveAll");
                _vsInstamce.Solution.Close(true);
            }
            _vsInstamce?.Quit();
        }

        public static bool AddFilesToSolution(string solution, string projectName, string[] files)
        {
            Debug.Assert(files?.Length > 0);
            OpenVisualStudion(solution);
            try
            {
                if(_vsInstamce != null)
                {
                    if (!_vsInstamce.Solution.IsOpen) _vsInstamce.Solution.Open(solution);
                    else _vsInstamce.ExecuteCommand("File.SaveAll");

                    foreach(EnvDTE.Project project in _vsInstamce.Solution.Projects)
                    {
                        if (project.UniqueName.Contains(projectName))
                        {
                            foreach(var file in files)
                            {
                                project.ProjectItems.AddFromFile(file);
                            }
                        }
                    }
                    var cpp = files.FirstOrDefault(x => Path.GetExtension(x) == ".cpp");
                    if (!string.IsNullOrEmpty(cpp))
                    {
                        _vsInstamce.ItemOperations.OpenFile(cpp, EnvDTE.Constants.vsViewKindTextView).Visible = true;
                    }
                    _vsInstamce.MainWindow.Activate();
                    _vsInstamce.MainWindow.Visible = true;
                }
            }catch(Exception ex)
            {
                Debug.WriteLine(ex.Message); ;
                Logger.Log(MessageType.Error, "Faild to Add files to the visual studio project : \n" + ex.Message);
                return false;
            }
            return true;
        }
    }
}
