using System;
using System.Collections.Generic;
using UnityEngine;

namespace YsoCorp {
    namespace GameUtils {

        [DefaultExecutionOrder(-15)]
        public class YCDataManager : BaseManager {
            private static string PLAYER_LAUNCH_COUNT = "YC_PLAYER_LAUNCH_COUNT";
            private static string PLAYER_GAME_COUNT = "YC_PLAYER_GAME_COUNT";

            private static string ADVERTISING_ID = "ADVERTISING_ID";
            private static string IOS_CONSENT_SHOWN = "IOS_CONSENT_SHOWN";
            private static string ADSSHOW = "ADSSHOW";
            private static string AB_PLAYER_SAMPLE = "YC_PLAYER_SAMPLE";
            private static string REDROCK_REVENUE = "REDROCK_REVENUE";
            private static string LANGUAGE = "LANGUAGE";

            private static string GDPR_ADS = "GDPR_ADS";
            private static string GDPR_ANALYTICS = "GDPR_ANALYTICS";
            private static string INTERSTITIALS_NB = "INTERSTITIALS_NB";
            private static string REWARDEDS_NB = "REWARDEDS_NB";
            private static string TIMESTAMP = "TIMESTAMP";

            private static string MMP_EVENT = "MMP_EVENT_";

            private int[] _interstitialsWatchedMilestones = new int[] { 10, 15, 20 };
            private int[] _rewardedsWatchedMilestones = new int[] { 2, 5, 7 };
            private int[] _levelsReachedMilestones = new int[] { 3, 5, 7, 10, 15, 20, 25, 30, 40, 50, 100, 150, 200, 250, 300, 400, 500 };

            private void Awake() {
                this.ycManager.dataManager = this;
                if (ADataManager.HasKey(TIMESTAMP) == false) {
                    ADataManager.SetInt(TIMESTAMP, this.GetTimestamp());
                }
            }

            public int GetTimestamp() {
                return (int)(DateTimeOffset.Now.ToUnixTimeMilliseconds() / 1000);
            }

            public int GetDiffTimestamp() {
                return this.GetTimestamp() - ADataManager.GetInt(TIMESTAMP);
            }

            private void CheckMmpEvent(int value, int[] milestones, string endString) {
                for (int i = 0; i < milestones.Length; i++) {
                    string fullName = value.ToString() + endString;
                    if (value == milestones[i] && this.IsMmpEventSent(fullName) == false) {
                        YCManager.instance.mmpManager.SendEvent(fullName);
                        this.SetMmpEventSent(fullName);
#if FIREBASE
                        Firebase.Analytics.FirebaseAnalytics.LogEvent("yc_" + fullName);
#endif
                        return;
                    }
                }
            }

            //PLAYER
            public void SetPlayerLaunchCount(int amount) {
                ADataManager.SetInt(PLAYER_LAUNCH_COUNT, amount);
            }

            public void IncrementPlayerLaunchCount() {
                this.SetPlayerLaunchCount(this.GetPlayerLaunchCount() + 1);
            }

            public int GetPlayerLaunchCount() {
                return ADataManager.GetInt(PLAYER_LAUNCH_COUNT);
            }

            public void SetPlayerGameCount(int amount) {
                ADataManager.SetInt(PLAYER_GAME_COUNT, amount);
            }

            public void IncrementPlayerGameCount() {
                this.SetPlayerGameCount(this.GetPlayerGameCount() + 1);
            }

            public int GetPlayerGameCount() {
                return ADataManager.GetInt(PLAYER_GAME_COUNT);
            }

            //ADVERTISING ID
            public string GetAdvertisingId() {
                return ADataManager.GetString(ADVERTISING_ID, "");
            }
            public void SetAdvertisingId(string id) {
                ADataManager.SetString(ADVERTISING_ID, id);
            }
            public bool GetIosConsentFlowShown() {
                return ADataManager.GetBool(IOS_CONSENT_SHOWN);
            }
            public void SetIosConsentFlowShown() {
                ADataManager.SetBool(IOS_CONSENT_SHOWN, true);
            }

            // ABTEST
            public void SetPlayerSample(string sample) {
                ADataManager.SetString(AB_PLAYER_SAMPLE, sample);
            }
            public string GetPlayerSample() {
                return ADataManager.GetString(AB_PLAYER_SAMPLE);
            }
            public bool HasPlayerSample() {
                return ADataManager.HasKey(AB_PLAYER_SAMPLE);
            }
            public void DeletePlayerSample() {
                ADataManager.DeleteKey(AB_PLAYER_SAMPLE);
            }

            // ADS
            public bool GetAdsShow() {
                return ADataManager.GetInt(ADSSHOW, 1) == 1;
            }
            public void BuyAdsShow() {
                ADataManager.SetInt(ADSSHOW, 0);
            }

            // AdsManager
            public Dictionary<string, double> GetRedRockRevenue() {
                return ADataManager.GetObject<Dictionary<string, double>>(REDROCK_REVENUE);
            }

            public void SetRedRockRevenue(Dictionary<string, double> dict) {
                ADataManager.SetObject(REDROCK_REVENUE, dict);
            }

            // LANGUAGE
            public void SetLanguage(string lang) {
                ADataManager.SetString(LANGUAGE, lang);
            }
            public string GetLanguage() {
                return ADataManager.GetString(LANGUAGE, "EN");
            }
            public bool HasLanguage() {
                return ADataManager.HasKey(LANGUAGE);
            }

            // GDPR
            public void SetGdprAds(bool consent) {
                ADataManager.SetBool(GDPR_ADS, consent);
            }
            public bool GetGdprAds() {
                return ADataManager.GetBool(GDPR_ADS, true);
            }

            public void SetGdprAnalytics(bool analytics) {
                ADataManager.SetBool(GDPR_ANALYTICS, analytics);
            }
            public bool GetGdprAnalytics() {
                return ADataManager.GetBool(GDPR_ANALYTICS, true);
            }

            // NB INTERSTITIALS
            public int GetInterstitialsNb() {
                return ADataManager.GetInt(INTERSTITIALS_NB, 0);
            }
            public void IncrementInterstitialsNb() {
                int newValue = this.GetInterstitialsNb() + 1;
                ADataManager.SetInt(INTERSTITIALS_NB, newValue);
                this.CheckMmpEvent(newValue, this._interstitialsWatchedMilestones, "_interstitials_watched");
            }

            // NB REWARDEDS
            public int GetRewardedsNb() {
                return ADataManager.GetInt(REWARDEDS_NB, 0);
            }
            public void IncrementRewardedsNb() {
                int newValue = this.GetRewardedsNb() + 1;
                ADataManager.SetInt(REWARDEDS_NB, newValue);
                this.CheckMmpEvent(newValue, this._rewardedsWatchedMilestones, "_rewardeds_watched");
            }

            // LEVEL PLAYED
            public void IncrementLevelPlayed(int level, bool win) {
                if (win) {
                    this.CheckMmpEvent(level, this._levelsReachedMilestones, "_level_reached");
                }

#if FIREBASE
                Firebase.Analytics.FirebaseAnalytics.LogEvent("yc_level_finished");
#endif
            }

            //MMP EVENTS
            public bool IsMmpEventSent(string name) {
                return ADataManager.GetBool(MMP_EVENT + name);
            }

            public void SetMmpEventSent(string name, bool value = true) {
                ADataManager.SetBool(MMP_EVENT + name, value);
            }

        }
    }
}
