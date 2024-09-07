using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

#if IN_APP_PURCHASING
using Unity.Services.Core;
using Unity.Services.Core.Environments;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Extension;
#endif

namespace YsoCorp {
    namespace GameUtils {

        [DefaultExecutionOrder(-15)]
        public class InAppManager : BaseManager
#if IN_APP_PURCHASING
            , IDetailedStoreListener
#endif
            {

#if IN_APP_PURCHASING
            private Dictionary<string, UnityEvent> _onPurchased = new Dictionary<string, UnityEvent>();
            private Dictionary<string, UnityEvent<PurchaseFailureDescription>> _onPurchaseFailed = new Dictionary<string, UnityEvent<PurchaseFailureDescription>>();

            /// <summary>
            /// Called when the Unity IAP system successfully finished initiating.
            /// </summary>
            [HideInInspector] public UnityEvent OnIAPInit = new UnityEvent();

            private IStoreController _StoreController = null;
            private IExtensionProvider _StoreExtensionProvider = null;
#endif

            private void Awake() {
                this.ycManager.inAppManager = this;
            }

#if IN_APP_PURCHASING
            async void Start() {
                try {
                    var options = new InitializationOptions().SetEnvironmentName("production");

                    await UnityServices.InitializeAsync(options);
                    this.Init();
                } catch (Exception exception) {
                    this.DebugLogError("Service initialization failed with error : " + exception.Message);
                }
            }

            public void Init() {
                if (this.IsInitialized()) {
                    return;
                }
                var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());
                if (this.ycManager.ycConfig.InAppRemoveAds != "") {
                    builder.AddProduct(this.ycManager.ycConfig.InAppRemoveAds, ProductType.NonConsumable);
                }
                foreach (CustomInapp inapp in this.ycManager.ycConfig.CustomInapps) {
                    ProductType type = inapp.isConsumable ? ProductType.Consumable : ProductType.NonConsumable;
                    builder.AddProduct(inapp.inappKey, type);
                }
                UnityPurchasing.Initialize(this, builder);
            }

            public bool IsInitialized() {
                return this._StoreController != null && this._StoreExtensionProvider != null;
            }

            public Product GetProductById(string productId) {
                if (this._StoreController != null && this._StoreController.products != null) {
                    return this._StoreController.products.WithID(productId);
                }
                return null;
            }
#endif
            /// <summary>
            /// Get the price of an IAP product as a string, including the currency symbol.
            /// </summary>
            /// <param name="productId">The product ID</param>
            /// <returns>Returns the price of the product as a string</returns>
            public string GetProductPrice(string productId) {
#if IN_APP_PURCHASING
                Product p = this.GetProductById(productId);
                if (p != null) {
                    return p.metadata.localizedPriceString;
                }
#endif
                return "";
            }

#if IN_APP_PURCHASING
            [Obsolete("Obsolete since v1.48.0 and will be removed. Please use AddListener without the bool parameter or AddListenerOnFailed")]
            public void AddListener(string productId, UnityAction onPurchase, bool purchaseSucceeded) {
                if (purchaseSucceeded) {
                    this.AddListener(productId, onPurchase);
                } else {
                    this.AddListenerOnFailed(productId, (failure) => onPurchase?.Invoke());
                }
            }

            /// <summary>
            /// Adds an action to a product ID to be executed when the purchase is completed.
            /// </summary>
            /// <param name="productId">The product ID</param>
            /// <param name="onPurchase">The action to be executed</param>
            public void AddListener(string productId, UnityAction onPurchase) {
                if (this.IsProductIdValid(productId)) {
                    if (this._onPurchased.ContainsKey(productId) == false) {
                        this._onPurchased[productId] = new UnityEvent();
                    }
                    this._onPurchased[productId].AddListener(onPurchase);
                }
            }

            /// <summary>
            /// Adds an action to a product ID to be executed when the purchase failed.
            /// </summary>
            /// <param name="productId">The product ID</param>
            /// <param name="onPurchaseFailed">The action to be executed</param>
            public void AddListenerOnFailed(string productId, UnityAction<PurchaseFailureDescription> onPurchaseFailed) {
                if (this.IsProductIdValid(productId)) {
                    if (this._onPurchaseFailed.ContainsKey(productId) == false) {
                        this._onPurchaseFailed[productId] = new UnityEvent<PurchaseFailureDescription>();
                    }
                    this._onPurchaseFailed[productId].AddListener(onPurchaseFailed);
                }
            }

            [Obsolete("Obsolete since v1.48.0 and will be removed. Please use RemoveListener without the bool parameter or RemoveListenerOnFailed")]
            public void RemoveListener(string productId, UnityAction onPurchase, bool purchaseSucceeded) {
                if (purchaseSucceeded) {
                    this.RemoveListener(productId, onPurchase);
                } else {
                    this.RemoveListenerOnFailed(productId, (failure) => onPurchase?.Invoke());
                }
            }

            /// <summary>
            /// Removes an action previously added with AddListener to a product ID.
            /// </summary>
            /// <param name="productId">The product ID</param>
            /// <param name="onPurchase">The action to be removed</param>
            public void RemoveListener(string productId, UnityAction onPurchase) {
                if (this.HasListener(productId, true) == false) return;

                UnityEvent eve = this._onPurchased[productId];
                eve?.RemoveListener(onPurchase);
            }

            /// <summary>
            /// Removes an action previously added with AddListenerOnFailed to a product ID.
            /// </summary>
            /// <param name="productId">The product ID</param>
            /// <param name="onPurchaseFailed">The action to be removed</param>
            public void RemoveListenerOnFailed(string productId, UnityAction<PurchaseFailureDescription> onPurchaseFailed) {
                if (this.HasListener(productId, false) == false) return;

                UnityEvent<PurchaseFailureDescription> eve = this._onPurchaseFailed[productId];
                eve?.RemoveListener(onPurchaseFailed);
            }

            [Obsolete("Obsolete since v1.48.0 and will be removed. Please use RemoveListener without the bool parameter or RemoveListenerOnFailed")]
            public void RemoveAllListener(string productId, bool purchaseSucceeded) {
                if (purchaseSucceeded) {
                    this.RemoveAllListener(productId);
                } else {
                    this.RemoveAllListenersOnFailed(productId);
                }
            }

            /// <summary>
            /// Removes all actions previously added with AddListener to a product ID. 
            /// </summary>
            /// <param name="productId">The product ID</param>
            public void RemoveAllListener(string productId) {
                if (this.HasListener(productId, true) == false) return;
                this._onPurchased[productId].RemoveAllListeners();
            }

            /// <summary>
            /// Removes all actions previously added with AddListenerOnFailed to a product ID.
            /// </summary>
            /// <param name="productId">The product ID</param>
            public void RemoveAllListenersOnFailed(string productId) {
                if (this.HasListener(productId, false) == false) return;
                this._onPurchaseFailed[productId].RemoveAllListeners();
            }

            /// <summary>
            /// Checks if any action has been previously added via AddListener or AddListenerOnFailed. 
            /// </summary>
            /// <param name="productId">The product ID</param>
            /// <param name="purchaseSucceeded">Wheteher to check for actions from AddListener or AddListenerOnFailed</param>
            /// <returns>Returns true if any action has been previously added via AddListener or AddListenerOnFailed</returns>
            public bool HasListener(string productId, bool purchaseSucceeded = true) {
                if (this.IsProductIdValid(productId)) {
                    if (purchaseSucceeded) {
                        return this._onPurchased.ContainsKey(productId);
                    } else {
                        return this._onPurchaseFailed.ContainsKey(productId);
                    }
                }
                return false;
            }
#endif

            /// <summary>
            /// Displays the store popup to confirm the purchase.
            /// </summary>
            /// <param name="productId">The product ID</param>
            public void BuyProductID(string productId) {
#if IN_APP_PURCHASING
                if (this.IsInitialized()) {
                    Product product = this.GetProductById(productId);
                    if (product != null && product.availableToPurchase) {
                        this.DebugLog(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                        this._StoreController.InitiatePurchase(product);
                    } else {
                        this.DebugLogError("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");
                    }
                } else {
                    this.DebugLogError("BuyProductID FAIL. Not initialized.");
                }
#endif
            }

            /// <summary>
            /// Restores the purchases previously made by a user.
            /// </summary>
            public void RestorePurchases() {
#if IN_APP_PURCHASING
                if (!this.IsInitialized()) {
                    this.DebugLogError("RestorePurchases FAIL. Not initialized.");
                    return;
                }

                if (Application.platform == RuntimePlatform.IPhonePlayer || Application.platform == RuntimePlatform.OSXPlayer) {
                    this.DebugLog("RestorePurchases started ...");
                    var apple = this._StoreExtensionProvider.GetExtension<IAppleExtensions>();
                    apple.RestoreTransactions((result, error) => {
                        if (result) {
                            this.DebugLog("RestorePurchases continuing. If no further messages, no purchases available to restore.");
                        } else {
                            this.DebugLog("RestorePurchases failed: " + error);
                        }
                    });
                } else {
                    this.DebugLogError("RestorePurchases FAIL. Not supported on this platform. Current = " + Application.platform);
                }
#endif
            }

#if IN_APP_PURCHASING
            public void OnInitialized(IStoreController controller, IExtensionProvider extensions) {
                this.DebugLog("OnInitialized: PASS");
                this._StoreController = controller;
                this._StoreExtensionProvider = extensions;
                this.OnIAPInit.Invoke();
            }

            public void OnInitializeFailed(InitializationFailureReason error) {
                this.DebugLogError("OnInitializeFailed InitializationFailureReason:" + error);
            }

            public void OnInitializeFailed(InitializationFailureReason error, string? message) {
                this.DebugLogError("OnInitializeFailed InitializationFailureReason:" + error);
            }

            public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs args) {
                this.ycManager.mmpManager.tenjinManager.SendTenjinPurchaseEvent(args);
                string productId = args.purchasedProduct.definition.id;
                Product p = this.GetProductById(productId);
                if (p != null) {
                    float price = (float)p.metadata.localizedPrice;
                    string isoCurrencyCode = p.metadata.isoCurrencyCode;
                    this.ycManager.analyticsManager.InAppBought(productId, price, isoCurrencyCode);
                }
                this._onPurchased[productId]?.Invoke();

                return PurchaseProcessingResult.Complete;
            }

            public void OnPurchaseFailed(Product product, PurchaseFailureReason failureReason) {
                this.DebugLogError(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureReason: {1}", product.definition.storeSpecificId, failureReason));
            }

            public void OnPurchaseFailed(Product product, PurchaseFailureDescription failureDescription) {
                this.DebugLogError(string.Format("OnPurchaseFailed: FAIL. Product: '{0}', PurchaseFailureDescription: {1}", product.definition.storeSpecificId, failureDescription.message));
                this._onPurchaseFailed[failureDescription.productId].Invoke(failureDescription);
            }
#endif

            /// <summary>
            /// Displays the store popup to confirm the purchase of the "No Ads" product.
            /// </summary>
            public void BuyProductIDAdsRemove() {
                this.BuyProductID(this.ycManager.ycConfig.InAppRemoveAds);
            }

            /// <summary>
            /// Checks if the product ID is included in the YCConfigData file.
            /// </summary>
            /// <param name="productId">The product ID</param>
            /// <returns>Returns true if the product ID is included in the YCConfigData file</returns>
            public bool IsProductIdValid(string productId) {
#if IN_APP_PURCHASING
                if (productId.CompareTo(this.ycManager.ycConfig.InAppRemoveAds) == 0) {
                    return true;
                }
                foreach (CustomInapp inapp in this.ycManager.ycConfig.CustomInapps) {
                    if (inapp.inappKey.CompareTo(productId) == 0) {
                        return true;
                    }
                }
                this.DebugLogError("The InApp key: " + productId + " does not exist in the YCConfig list");
                return false;
#else
                return true;
#endif
            }

            private void DebugLog(object message) {
                if (this.ycManager.ycConfig.InappDebug) {
                    Debug.Log("[GameUtils - Inapps] " + message);
                }
            }
            private void DebugLogError(object message) {
                Debug.LogError("[GameUtils - Inapps] " + message);
            }

            [Serializable]
            public struct CustomInapp {
                public string inappKey;
                public bool isConsumable;

                public CustomInapp(string inappKey, bool isConsumable) {
                    this.inappKey = inappKey;
                    this.isConsumable = isConsumable;
                }
            }

        }
    }
}