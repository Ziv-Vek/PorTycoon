using UnityEngine;
using System.Collections.Generic;

namespace YsoCorp {

    namespace GameUtils {

        [DefaultExecutionOrder(-15)]
        public class ABTestingManager : BaseManager {

            private Dictionary<string, bool> _results = new Dictionary<string, bool>();
            private string _sample = null;

            private void Awake() {
                this.ycManager.abTestingManager = this;
            }

            private void Start() {
                if (this.ycManager.ycConfig.ABDebugLog) {
                    Debug.Log("[AB Testing] : " + this.GetPlayerSample());
                }
            }

            private void SetSample(string sample) {
                if (sample == null) {
                    if (this.ycManager.dataManager.HasPlayerSample()) {
                        this.ycManager.dataManager.DeletePlayerSample();
                    }
                } else {
                    this.ycManager.dataManager.SetPlayerSample(sample);
                }
            }

            private string GetSample() {
                return this.ycManager.dataManager.GetPlayerSample();
            }

            private bool IsSample() {
                return this.ycManager.dataManager.HasPlayerSample();
            }

            private string ConvertSample(string sample, bool allVersion = false) {
                if (allVersion) {
                    return sample.Trim();
                }
                return this.ycManager.ycConfig.ABVersion + "-" + sample.Trim();
            }

            private float GetABPercent() {
                return 1f / this.GetABSamples().Length;
            }

            private string[] GetABSamples() {
                List<string> abs = new List<string>();
                if (this.ycManager.ycConfig.ABSamples.Length > 0) {
                    abs.Add(this.ConvertSample("control"));
                    foreach (string ab in this.ycManager.ycConfig.ABSamples) {
                        abs.Add(this.ConvertSample(ab));
                    }
                }
                return abs.ToArray();
            }

            private void GenerateSample() {
                if (this.IsSample() == false) {
                    string[] abSamples = this.GetABSamples();
                    float r = Random.value;
                    string sample = "";
                    for (int i = 0; i < abSamples.Length; i++) {
                        if (r < (i + 1) * this.GetABPercent()) {
                            sample = abSamples[i];
                            break;
                        }
                    }
                    this.SetSample(sample);
                }
            }

            // PUBLIC

            /// <summary>
            /// Get the name of the current AB Test.
            /// </summary>
            /// <returns>Returns a the AB Test as a string.</returns>
            public string GetPlayerSample() {
                if (this._sample == null) {
                    this.GenerateSample();
                    this._sample = this.GetSample();
                    if ((Debug.isDebugBuild || Application.isEditor) && this.ycManager.ycConfig.ABForcedSample != "") {
                        this._sample = this.ConvertSample(this.ycManager.ycConfig.ABForcedSample);
                    }
                }
                return this._sample;
            }

            /// <summary>
            /// Tests if the given string exactly matches the current AB Test.
            /// </summary>
            /// <param name="name">The name of the AB Test</param>
            /// <returns>Returns true if the current AB Test exactly matches the given one</returns>
            public bool IsPlayerSample(string name) {
                if (this._results.ContainsKey(name) == false) {
                    this._results[name] = this.GetPlayerSample() == this.ConvertSample(name);
                }
                return this._results[name];
            }

            /// <summary>
            /// Tests if the current AB Test contains the given string.
            /// </summary>
            /// <param name="name">The name or partial name of the AB Test</param>
            /// <returns>Returns true if the current AB Test contains the given one</returns>
            public bool IsPlayerSampleContains(string name) {
                return this.GetPlayerSample().StartsWith(this.ycManager.ycConfig.ABVersion + "-") && this.GetPlayerSample().Contains(name);
            }
        }

    }
}
