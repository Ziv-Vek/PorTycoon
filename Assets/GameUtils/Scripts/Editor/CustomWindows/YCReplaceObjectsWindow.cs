using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace YsoCorp {
    namespace GameUtils {

        public class YCReplaceObjectsWindow : YCCustomWindow {

            private static GameObject gameObjectModelReplacing;
            private bool _useScale = false;
            private bool _useName = false;

            public void Init() {
                this.SetMinSize(400, 120);
            }

            private void OnGUI() {
                this.AddLabel("Replace all currently selected GameObjects with the one below.\n");
                gameObjectModelReplacing = this.AddObjectField<GameObject>("New GameObject", gameObjectModelReplacing, true);

                List<GameObject> gameObjectsToReplace = new List<GameObject>();
                foreach (GameObject selectedGameObject in Selection.gameObjects) {
                    if (EditorUtility.IsPersistent(selectedGameObject)) {
                        continue;
                    }

                    gameObjectsToReplace.Add(selectedGameObject);
                }

                this._useScale = this.AddToggle("Override Scale", this._useScale);
                this._useName = this.AddToggle("Override Name", this._useName);

                GUI.enabled = gameObjectModelReplacing != null && gameObjectsToReplace.Count > 0;
                this.AddButton("Replace", () => {
                    List<GameObject> createdReplacingGameObjects = new List<GameObject>();
                    this.ReplaceBy(gameObjectsToReplace, gameObjectModelReplacing, ref createdReplacingGameObjects, this._useScale, this._useName);
                    Selection.objects = createdReplacingGameObjects.ToArray();
                });
            }

            // Methods
            public void ReplaceBy(List<GameObject> gameObjectToReplaces, GameObject gameObjectModelReplacing, ref List<GameObject> createdGameObjects, bool scaleOverride = false, bool nameOverride = false) {
                if (gameObjectToReplaces == null || gameObjectToReplaces.Count <= 0 || gameObjectModelReplacing == null) {
                    return;
                }

                foreach (GameObject gameObjectToReplace in gameObjectToReplaces) {
                    GameObject createdGameObject = ReplaceBy(gameObjectToReplace, gameObjectModelReplacing, scaleOverride, nameOverride);
                    if (createdGameObject != null && createdGameObjects != null) {
                        createdGameObjects.Add(createdGameObject);
                    }
                }
            }

            public GameObject ReplaceBy(GameObject gameObjectToReplace, GameObject gameObjectModelReplacing, bool scaleOverride, bool nameOverride) {
                if (gameObjectToReplace == null || gameObjectModelReplacing == null) {
                    return null;
                }

                GameObject createdGameObjectReplacing = null;
                if (EditorUtility.IsPersistent(gameObjectModelReplacing)) {
                    createdGameObjectReplacing = PrefabUtility.InstantiatePrefab(gameObjectModelReplacing) as GameObject;
                }

                if (createdGameObjectReplacing == null) {
                    createdGameObjectReplacing = Object.Instantiate(gameObjectModelReplacing);
                }
                createdGameObjectReplacing.name = gameObjectToReplace.name;
                createdGameObjectReplacing.SetActive(gameObjectToReplace.activeSelf);
                createdGameObjectReplacing.transform.parent = gameObjectToReplace.transform.parent;
                createdGameObjectReplacing.transform.SetSiblingIndex(gameObjectToReplace.transform.GetSiblingIndex());
                createdGameObjectReplacing.transform.localPosition = gameObjectToReplace.transform.localPosition;
                createdGameObjectReplacing.transform.localRotation = gameObjectToReplace.transform.localRotation;
                createdGameObjectReplacing.transform.localScale = scaleOverride ? gameObjectModelReplacing.transform.localScale : gameObjectToReplace.transform.localScale;
                createdGameObjectReplacing.name = nameOverride ? gameObjectModelReplacing.name : gameObjectToReplace.name;

                string undoName = "Replace " + gameObjectToReplace + "By " + createdGameObjectReplacing;
                Undo.RegisterCreatedObjectUndo(createdGameObjectReplacing, undoName);
                Undo.RegisterFullObjectHierarchyUndo(gameObjectToReplace, undoName);
                Object.DestroyImmediate(gameObjectToReplace);

                return createdGameObjectReplacing;
            }
        }
    }
}
