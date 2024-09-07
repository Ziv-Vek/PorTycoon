using System.IO;
using UnityEditor;
using UnityEngine;

namespace YsoCorp {
    namespace GameUtils {

        public class YCGameutilsUpdateWindow : YCCustomWindow {

            private YCUpdatesHandler.GUUpdateData data;
            private bool useAdvancedSettings;
            private bool deleteObsoletes;

            public void Init(YCUpdatesHandler.GUUpdateData updateData) {
                this.data = updateData;

                this.useAdvancedSettings = false;
                this.deleteObsoletes = true;
                this.SetPosition(WindowPosition.MiddleCenter);
            }

            private void OnGUI() {
                string ok = "";
                GUIStyle style = new GUIStyle(GUI.skin.label);
                if (this.data.isUpToDate) {
                    style.normal.textColor = Color.green;
                    this.AddLabel("GameUtils is already up to date.", TextAnchor.MiddleCenter, style);
                    ok = "Reimport";
                } else {
                    style.normal.textColor = Color.red;
                    this.AddLabel("GameUtils needs to be updated.", TextAnchor.MiddleCenter, style);
                    ok = "Update";
                }
                this.AddEmptyLine();
                this.AddLabel("Do you want to import the version " + this.data.version + "?");
                this.useAdvancedSettings = this.AddToggle("Advanced settings", this.useAdvancedSettings);
                this.data.isAdvanced = this.useAdvancedSettings;
                if (this.useAdvancedSettings) {
                    this.deleteObsoletes = this.AddToggle("Delete obsolete files and folders before importing (recommended)", this.deleteObsoletes);
                    this.SetSize(450, 150);
                } else {
                    this.deleteObsoletes = true;
                    this.SetSize(450, 115);
                }
                this.AddEmptyLine();
                this.AddCancelOk("Cancel", ok, () => this.Close(), () => {
                    if (this.deleteObsoletes) {
                        foreach (string path in this.data.additionalRemovalsPaths) {
                            if (YCIOHandler.IsDirectory(path)) {
                                YCIOHandler.DeleteDirectory(path, true);
                            } else {
                                YCIOHandler.DeleteFile(path);
                            }
                        }
                        AssetDatabase.Refresh();
                    }
                    YCUpdatesHandler.ImportGameutilsPackage(this.data);
                    this.Close();
                });
            }
        }
    }
}
