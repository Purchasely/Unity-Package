#import <UserNotifications/UserNotifications.h>
#import <Purchasely/Purchasely-Swift.h>
#import "PLYUtils.h"

#pragma clang diagnostic push
#pragma ide diagnostic ignored "OCUnusedGlobalDeclarationInspection"

// Callback definitions
typedef void(PurchaselyStartCallbackDelegate)(void *actionPtr, bool success, const char *error);

typedef void(PurchaselyBoolCallbackDelegate)(void *actionPtr, bool refreshRequired);

typedef void(PurchaselyVoidCallbackDelegate)(void *actionPtr);

typedef void(PurchaselyPresentationResultCallbackDelegate)(void *actionPtr, int result, char* planJson);

typedef void(PurchaselyStringCallbackDelegate)(void *actionPtr, const char *eventJson);

typedef void(PurchaselySignatureCallbackDelegate)(void *actionPtr, const char *json, void* pointer);

typedef void(PurchaselyPresentationCallbackDelegate)(void *actionPtr, const char *json, void* pointer);

// Event Delegate
@interface PurchaselyEventDelegate : NSObject <PLYEventDelegate>

@property(nonatomic, copy) void (^eventCallback)(NSDictionary<NSString*, id>* properties);

@end

@implementation PurchaselyEventDelegate

- (void)eventTriggered:(enum PLYEvent)event properties:(NSDictionary<NSString *, id> * _Nullable)properties {
    if (_eventCallback == nil)
        return;

    NSMutableDictionary* dict = [[NSMutableDictionary alloc] initWithDictionary:properties];
    dict[@"name"] = [NSString fromPLYEvent:event];

    _eventCallback(dict);
}

@end

// Paywall Interceptor Delegate
@interface PurchaselyInterceptorDelegate : NSObject

@property(nonatomic, copy) void (^actionCallback)(char* actionJson);

@end

@implementation PurchaselyInterceptorDelegate

@end

// Bridge methods
extern "C" {
    PurchaselyEventDelegate* _eventDelegate;
    UINavigationController* presentedPresentationViewController;
    void (^onProcessActionHandler)(BOOL proceed);

    PurchaselyInterceptorDelegate* interceptorDelegate;
    void (^interceptorFunction)(enum PLYPresentationAction action, PLYPresentationActionParameters * _Nullable parameters, PLYPresentationInfo * _Nullable infos, void (^ _Nonnull onProcessActionHandler)(BOOL));

    void (actionInterceptorDelegate)(void *actionPtr, const char *eventJson);
    void* actionInterceptorCalbackPtr;

    void _purchaselyStart(const char* apiKey, const char* userId, int logLevel, int runningMode, bool storekit1,
                          PurchaselyStartCallbackDelegate startCallback, void* startCallbackPtr) {

        [Purchasely startWithAPIKey:[PLYUtils createNSStringFrom:apiKey]
                          appUserId:[PLYUtils createNSStringFrom:userId]
                        runningMode:[PLYUtils parseRunningMode:runningMode]
          paywallActionsInterceptor:nil
                   storekitSettings:(storekit1 ? [StorekitSettings storeKit1]: [StorekitSettings storeKit2])
                           logLevel:[PLYUtils parseLogLevel:logLevel]
                        initialized:^(BOOL success, NSError * _Nullable error) {
            NSString* errorString = error == nil ? @"" : [error localizedDescription];
            startCallback(startCallbackPtr, success, [PLYUtils createCStringFrom:errorString]);
        }];

        [Purchasely setAppTechnology:PLYAppTechnologyUnity];
    }

    void _purchaselyUserLogin(const char* userId, PurchaselyBoolCallbackDelegate onUserLogin, void* onUserLoginPtr) {
        [Purchasely userLoginWith:[PLYUtils createNSStringFrom:userId] shouldRefresh:^(BOOL shouldRefresh) {
            onUserLogin(onUserLoginPtr, shouldRefresh);
        }];
    }

    void _purchaselySetIsReadyToOpenDeeplink(bool ready) {
        [Purchasely readyToOpenDeeplink:ready];
    }

    void _purchaselySetDefaultPresentationResultHandler(PurchaselyPresentationResultCallbackDelegate presentationResultCallback, void*     presentationResultCallbackPtr) {
        [Purchasely setDefaultPresentationResultHandler:^(enum PLYProductViewControllerResult result, PLYPlan * _Nullable plan) {
            presentationResultCallback(presentationResultCallbackPtr, [PLYUtils parseProductViewResult:result], [PLYUtils planAsJson:plan]);
        }];
    }

    void showNavigationControllerForView(UIViewController* controller) {
        if (controller != nil) {
            presentedPresentationViewController = [[UINavigationController alloc] initWithRootViewController:controller];
            [presentedPresentationViewController.navigationBar setTranslucent:YES];
            [presentedPresentationViewController.navigationBar setBackgroundImage:[UIImage new] forBarMetrics:UIBarMetricsDefault];
            [presentedPresentationViewController.navigationBar setShadowImage: [UIImage new]];
            [presentedPresentationViewController.navigationBar setTintColor: [UIColor whiteColor]];
            presentedPresentationViewController.modalPresentationStyle = UIModalPresentationFullScreen;

            [Purchasely showController:presentedPresentationViewController type: PLYUIControllerTypeProductPage];
        }
    }

    void _purchaselyPresentPresentationWithId(const char* presentationId, const char* contentId, PurchaselyBoolCallbackDelegate loadCallback, void*     loadCallbackPtr, PurchaselyVoidCallbackDelegate closeCallback, void* closeCallbackPtr, PurchaselyPresentationResultCallbackDelegate     presentationResultCallback, void* presentationResultCallbackPtr) {

        auto loadedFunction = ^(PLYPresentationViewController * _Nullable constroller, BOOL loaded, NSError * _Nullable error) {
            if (error != nil) {
                NSLog(@"%@", [error localizedDescription]);
            }

            loadCallback(loadCallbackPtr, loaded);
        };

        auto completionFunction = ^(enum PLYProductViewControllerResult result, PLYPlan * _Nullable plan) {
            presentationResultCallback(presentationResultCallbackPtr, [PLYUtils parseProductViewResult:result], [PLYUtils planAsJson:plan]);
        };

        NSString* contentIdStr = [PLYUtils createNSStringFrom:contentId];
        if ([contentIdStr length] == 0)
            contentIdStr = nil;

        UIViewController* controller = [Purchasely presentationControllerWith:[PLYUtils createNSStringFrom:presentationId]
                                                                    contentId:contentIdStr
                                                                       loaded:loadedFunction
                                                                   completion:completionFunction];

        if (controller != nil) {
            showNavigationControllerForView(controller);
        } else {
            NSLog(@"Purchasely view is not valid. Will not show.");
        }
    }

    void _purchaselyPresentPresentationForPlacement(const char* placementId, const char* contentId, PurchaselyBoolCallbackDelegate loadCallback, void*     loadCallbackPtr, PurchaselyVoidCallbackDelegate closeCallback, void* closeCallbackPtr, PurchaselyPresentationResultCallbackDelegate     presentationResultCallback, void* presentationResultCallbackPtr) {

        auto loadedFunction = ^(PLYPresentationViewController * _Nullable constroller, BOOL loaded, NSError * _Nullable error) {
            if (error != nil) {
                NSLog(@"%@", [error localizedDescription]);
            }

            loadCallback(loadCallbackPtr, loaded);
        };

        auto completionFunction = ^(enum PLYProductViewControllerResult result, PLYPlan * _Nullable plan) {
            presentationResultCallback(presentationResultCallbackPtr, [PLYUtils parseProductViewResult:result], [PLYUtils planAsJson:plan]);
        };

        NSString* contentIdStr = [PLYUtils createNSStringFrom:contentId];
        if ([contentIdStr length] == 0)
            contentIdStr = nil;

        UIViewController* controller = [Purchasely presentationControllerFor:[PLYUtils createNSStringFrom:placementId]
                                                                   contentId:contentIdStr
                                                                      loaded:loadedFunction
                                                                  completion:completionFunction];

        if (controller != nil) {
            showNavigationControllerForView(controller);
        } else {
            NSLog(@"Purchasely view is not valid. Will not show.");
        }
    }

    void _purchaselyPresentPresentationForPlan(const char* planId, const char* presentationId, const char* contentId, PurchaselyBoolCallbackDelegate     loadCallback, void* loadCallbackPtr, PurchaselyVoidCallbackDelegate closeCallback, void* closeCallbackPtr,     PurchaselyPresentationResultCallbackDelegate presentationResultCallback, void* presentationResultCallbackPtr) {

        auto loadedFunction = ^(PLYPresentationViewController * _Nullable constroller, BOOL loaded, NSError * _Nullable error) {
            if (error != nil) {
                NSLog(@"%@", [error localizedDescription]);
            }

            loadCallback(loadCallbackPtr, loaded);
        };

        auto completionFunction = ^(enum PLYProductViewControllerResult result, PLYPlan * _Nullable plan) {
            presentationResultCallback(presentationResultCallbackPtr, [PLYUtils parseProductViewResult:result], [PLYUtils planAsJson:plan]);
        };

        NSString* contentIdStr = [PLYUtils createNSStringFrom:contentId];
        if ([contentIdStr length] == 0)
            contentIdStr = nil;

        NSString* presentationIdStr = [PLYUtils createNSStringFrom:presentationId];
        if ([presentationIdStr length] == 0)
            presentationIdStr = nil;

        UIViewController* controller = [Purchasely planControllerFor:[PLYUtils createNSStringFrom:planId]
                                                                with:presentationIdStr
                                                           contentId:contentIdStr
                                                              loaded:loadedFunction
                                                          completion:completionFunction];

        if (controller != nil) {
            showNavigationControllerForView(controller);
        } else {
            NSLog(@"Purchasely view is not valid. Will not show.");
        }
    }

    void _purchaselyPresentPresentationForProduct(const char* productId, const char* presentationId, const char* contentId, PurchaselyBoolCallbackDelegate     loadCallback, void* loadCallbackPtr, PurchaselyVoidCallbackDelegate closeCallback, void* closeCallbackPtr,     PurchaselyPresentationResultCallbackDelegate presentationResultCallback, void* presentationResultCallbackPtr) {

        auto loadedFunction = ^(PLYPresentationViewController * _Nullable constroller, BOOL loaded, NSError * _Nullable error) {
            if (error != nil) {
                NSLog(@"%@", [error localizedDescription]);
            }

            loadCallback(loadCallbackPtr, loaded);
        };

        auto completionFunction = ^(enum PLYProductViewControllerResult result, PLYPlan * _Nullable plan) {
            presentationResultCallback(presentationResultCallbackPtr, [PLYUtils parseProductViewResult:result], [PLYUtils planAsJson:plan]);
        };

        NSString* contentIdStr = [PLYUtils createNSStringFrom:contentId];
        if ([contentIdStr length] == 0)
            contentIdStr = nil;

        NSString* presentationIdStr = [PLYUtils createNSStringFrom:presentationId];
        if ([presentationIdStr length] == 0)
            presentationIdStr = nil;

        UIViewController* controller = [Purchasely productControllerFor:[PLYUtils createNSStringFrom:productId]
                                                                   with:presentationIdStr
                                                              contentId:contentIdStr
                                                                 loaded:loadedFunction
                                                             completion:completionFunction];

        if (controller != nil) {
            showNavigationControllerForView(controller);
        } else {
            NSLog(@"Purchasely view is not valid. Will not show.");
        }
    }

    void _purchaselyPresentSubscriptions() {
        UIViewController *ctrl = [Purchasely subscriptionsController];
        UINavigationController *navCtrl = [[UINavigationController alloc] initWithRootViewController:ctrl];
        ctrl.navigationItem.leftBarButtonItem = [[UIBarButtonItem alloc] initWithBarButtonSystemItem: UIBarButtonSystemItemDone target:navCtrl     action:@selector(close)];

        [Purchasely showController:navCtrl type: PLYUIControllerTypeSubscriptionList];
    }

    void _purchaselyPurchase(const char* planId, const char* offerId,
    PurchaselyStringCallbackDelegate successCallback,
    void* successCallbackPtr,
    PurchaselyStringCallbackDelegate errorCallback,
    void* errorCallbackPtr) {

        NSString *planIdStr = [PLYUtils createNSStringFrom:planId];
        NSString *offerIdStr = [PLYUtils createNSStringFrom:offerId];

        [Purchasely planWith:planIdStr success:^(PLYPlan * _Nonnull plan) {

            if (@available(iOS 12.2, macOS 12.0, tvOS 15.0, watchOS 8.0, *)) {

                NSString *storeOfferId = nil;
                for (PLYPromoOffer *promoOffer in plan.promoOffers) {
                    if ([promoOffer.vendorId isEqualToString:offerIdStr]) {
                        storeOfferId = promoOffer.storeOfferId;
                        break;
                    }
                }
                if (storeOfferId) {
                    [Purchasely purchaseWithPromotionalOfferWithPlan:plan
                                                           contentId:nil
                                                        storeOfferId:storeOfferId
                                                             success:^{
                        successCallback(successCallbackPtr, [PLYUtils planAsJson:plan]);
                    } failure:^(NSError * _Nonnull error) {
                        errorCallback(errorCallbackPtr, [PLYUtils createCStringFrom:error.localizedDescription]);
                    }];
                } else {
                    [Purchasely purchaseWithPlan:plan success:^{
                        successCallback(successCallbackPtr, [PLYUtils planAsJson:plan]);
                    } failure:^(NSError * _Nonnull error) {
                        errorCallback(errorCallbackPtr, [PLYUtils createCStringFrom:error.localizedDescription]);
                    }];
                }
            } else {
                [Purchasely purchaseWithPlan:plan success:^{
                    successCallback(successCallbackPtr, [PLYUtils planAsJson:plan]);
                } failure:^(NSError * _Nonnull error) {
                    errorCallback(errorCallbackPtr, [PLYUtils createCStringFrom:error.localizedDescription]);
                }];
            }
        } failure:^(NSError * _Nullable error) {
            errorCallback(errorCallbackPtr, [PLYUtils createCStringFrom:error.localizedDescription]);
        }];
    }

    void _purchaselyRestoreAllProducts(bool isSilent, PurchaselyStringCallbackDelegate successCallback, void* successCallbackPtr,     PurchaselyStringCallbackDelegate errorCallback, void* errorCallbackPtr) {

        auto successAction = ^{
            successCallback(successCallbackPtr, [PLYUtils createCStringFrom:@"{}"]);
        };

        auto errorAction = ^(NSError * _Nonnull error) {
            errorCallback(errorCallbackPtr, [PLYUtils createCStringFrom:error.localizedDescription]);
        };

        if (isSilent) {
            [Purchasely synchronizeWithSuccess:successAction
                                       failure:errorAction];
        } else {
            [Purchasely restoreAllProductsWithSuccess:successAction failure:errorAction];
        }
    }

    void _purchaselyGetAllProducts(PurchaselyStringCallbackDelegate successCallback, void* successCallbackPtr, PurchaselyStringCallbackDelegate     errorCallback, void* errorCallbackPtr) {
        [Purchasely allProductsWithSuccess:^(NSArray<PLYProduct*>* _Nonnull products) {
            successCallback(successCallbackPtr, [PLYUtils productsAsJson:products]);
        } failure:^(NSError * _Nullable error) {
            errorCallback(errorCallbackPtr, [PLYUtils createCStringFrom:error.localizedDescription]);
        }];
    }

    void _purchaselyGetProduct(const char* productId, PurchaselyStringCallbackDelegate successCallback, void* successCallbackPtr,     PurchaselyStringCallbackDelegate errorCallback, void* errorCallbackPtr) {

        [Purchasely productWith:[PLYUtils createNSStringFrom:productId] success:^(PLYProduct * _Nonnull product) {
            successCallback(successCallbackPtr, [PLYUtils productAsJson:product]);
        } failure:^(NSError * _Nullable error) {
            errorCallback(errorCallbackPtr, [PLYUtils createCStringFrom:error.localizedDescription]);
        }];
    }

    void _purchaselyGetPlan(const char* planId, PurchaselyStringCallbackDelegate successCallback, void* successCallbackPtr,     PurchaselyStringCallbackDelegate errorCallback, void* errorCallbackPtr) {

        [Purchasely planWith:[PLYUtils createNSStringFrom:planId] success:^(PLYPlan * _Nonnull plan) {
            successCallback(successCallbackPtr, [PLYUtils planAsJson:plan]);
        } failure:^(NSError * _Nullable error) {
            errorCallback(errorCallbackPtr, [PLYUtils createCStringFrom:error.localizedDescription]);
        }];
    }

    void _purchaselyGetUserSubscriptions(PurchaselyStringCallbackDelegate successCallback, void* successCallbackPtr, PurchaselyStringCallbackDelegate     errorCallback, void* errorCallbackPtr) {
        [Purchasely userSubscriptions: false success:^(NSArray<PLYSubscription*>* _Nullable subscriptions) {
            successCallback(successCallbackPtr, [PLYUtils susbscriptionsAsJson:subscriptions]);
        } failure:^(NSError * _Nonnull error) {
            errorCallback(errorCallbackPtr, [PLYUtils createCStringFrom:error.localizedDescription]);
        }];
    }

    void _purchaselySynchronize(const char* urlString) {
        auto successAction = ^{
        };

        auto errorAction = ^(NSError * _Nonnull error) {
        };

        [Purchasely synchronizeWithSuccess:successAction
                                       failure:errorAction];
    }

    bool _purchaselyIsDeeplinkHandled(const char* urlString) {
        NSURL* url = [NSURL URLWithString:[PLYUtils createNSStringFrom:urlString]];
        return [Purchasely isDeeplinkHandledWithDeeplink:url];
    }

    bool _purchaselyIsAnonymous() {
        return [Purchasely isAnonymous];
    }

    void _purchaselySignPromotionalOffer(const char* storeProductId,
                                         const char* storeOfferId,
                                         PurchaselySignatureCallbackDelegate successCallback,
                                         void* successCallbackPtr,
                                         PurchaselyStringCallbackDelegate errorCallback,
                                         void* errorCallbackPtr) {

        NSString *storeOfferIdStr = [PLYUtils createNSStringFrom:storeOfferId];
        NSString *storeProductIdStr = [PLYUtils createNSStringFrom:storeProductId];

        dispatch_async(dispatch_get_main_queue(), ^{

            if (@available(iOS 12.2, *)) {
                [Purchasely signPromotionalOfferWithStoreProductId:storeProductIdStr storeOfferId:storeOfferIdStr success:^(PLYOfferSignature * _Nonnull signature) {

                    successCallback(successCallbackPtr, [PLYUtils signatureToJson:signature], errorCallbackPtr);
                } failure:^(NSError * _Nullable error) {
                    errorCallback(errorCallbackPtr, [PLYUtils createCStringFrom:error.localizedDescription]);
                }];
            } else {
                errorCallback(errorCallbackPtr, nil);
            }
        });
    }

    void _purchaselyIsEligibleForIntroOffer(const char* planVendorId,
                                             PurchaselyBoolCallbackDelegate successCallback,
                                             void* successCallbackPtr,
                                             PurchaselyStringCallbackDelegate errorCallback,
                                             void* errorCallbackPtr) {

        NSString *planVendorIdStr = [PLYUtils createNSStringFrom:planVendorId];

        dispatch_async(dispatch_get_main_queue(), ^{
            [Purchasely planWith:planVendorIdStr
                         success:^(PLYPlan * _Nonnull plan) {
                [plan isUserEligibleForIntroductoryOfferWithCompletion:^(BOOL isEligible) {
                    dispatch_async(dispatch_get_main_queue(), ^{
                        successCallback(successCallbackPtr, isEligible);
                    });
                }];
            } failure:^(NSError * _Nullable error) {
                errorCallback(errorCallbackPtr, [PLYUtils createCStringFrom:error.localizedDescription]);
            }];
        });
    }

    void _purchaselySetLanguage(const char* language) {
        NSLocale *locale = [NSLocale localeWithLocaleIdentifier:[PLYUtils createNSStringFrom:language]];
        [Purchasely setLanguageFrom:locale];
    }

    void _purchaselyClosePresentation() {
        if (presentedPresentationViewController != nil) {
            dispatch_async(dispatch_get_main_queue(), ^{
                [presentedPresentationViewController dismissViewControllerAnimated:true completion:^{
                    presentedPresentationViewController = nil;
                }];
            });
        }
    }

    void _purchaselyHidePresentation() {
        if (presentedPresentationViewController != nil) {
            dispatch_async(dispatch_get_main_queue(), ^{
                [presentedPresentationViewController dismissViewControllerAnimated:true completion:^{ }];
            });
        }
    }

    void _purchaselyShowPresentation() {
        if (presentedPresentationViewController != nil) {
            dispatch_async(dispatch_get_main_queue(), ^{
                showNavigationControllerForView(presentedPresentationViewController);
            });
        }
    }

    void _purchaselySetPaywallActionInterceptor(PurchaselyStringCallbackDelegate actionCallback, void* actionCallbackPtr) {
        interceptorDelegate = [PurchaselyInterceptorDelegate new];
        interceptorDelegate.actionCallback = ^(char* data) {
            actionCallback(actionCallbackPtr, data);
        };

        interceptorFunction = ^(enum PLYPresentationAction action, PLYPresentationActionParameters * _Nullable parameters,     PLYPresentationInfo * _Nullable infos, void (^ _Nonnull actionHandler)(BOOL)) {

            onProcessActionHandler = actionHandler;
            interceptorDelegate.actionCallback([PLYUtils actionToJson:action parameters:parameters presentationInfos:infos]);
        };

        [Purchasely setPaywallActionsInterceptor:interceptorFunction];
    }

    void _purchaselyProcessAction(bool process) {
        if (onProcessActionHandler != nil) {
            onProcessActionHandler(process);
        }
    }

    void _purchaselyUserDidConsumeSubscriptionContent() {
        [Purchasely userDidConsumeSubscriptionContent];
    }

    char* _purchaselyGetAnonymousUserId() {
        return [PLYUtils createCStringFrom:[Purchasely anonymousUserId]];
    }

    void _purchaselyUserLogout() {
        [Purchasely userLogout];
    }

    void _purchaselySetAttribute(int attribute, const char* value) {
        [Purchasely setAttribute:(PLYAttribute)attribute value:[PLYUtils createNSStringFrom:value]];
    }

    void _purchaselySetStringAttribute(const char* key, const char* value) {
        [Purchasely setUserAttributeWithStringValue:[PLYUtils createNSStringFrom:value] forKey:[PLYUtils createNSStringFrom:key]];
    }

    void _purchaselySetBoolAttribute(const char* key, bool value) {
        [Purchasely setUserAttributeWithBoolValue:value forKey:[PLYUtils createNSStringFrom:key]];
    }

    void _purchaselySetIntAttribute(const char* key, int value) {
        [Purchasely setUserAttributeWithIntValue:value forKey:[PLYUtils createNSStringFrom:key]];
    }

    void _purchaselySetFloatAttribute(const char* key, float value) {
        [Purchasely setUserAttributeWithDoubleValue:value forKey:[PLYUtils createNSStringFrom:key]];
    }

    void _purchaselySetDateAttribute(const char* key, const char* dateString) {
        NSDateFormatter *dateFormat = [[NSDateFormatter alloc] init];
        [dateFormat setDateFormat:@"yyyy-MM-dd'T'HH:mm:ssZ"];

        NSDate* date = [dateFormat dateFromString:[PLYUtils createNSStringFrom:dateString]];
        [Purchasely setUserAttributeWithDateValue:date forKey:[PLYUtils createNSStringFrom:key]];
    }

    char* _purchaselyGetUserAttribute(const char* key) {
        id attribute = [Purchasely getUserAttributeFor:[PLYUtils createNSStringFrom:key]];
        NSString* result = @"";
        if (attribute == nil)
            return [PLYUtils createCStringFrom: result];

        result = [NSString stringWithFormat:@"%@", attribute];
        return [PLYUtils createCStringFrom: result];
    }

    void _purchaselyClearAttribute(const char* key) {
        [Purchasely clearUserAttributeForKey:[PLYUtils createNSStringFrom:key]];
    }

    void _purchaselyClearAttributes() {
        [Purchasely clearUserAttributes];
    }

    void _purchaselySetThemeMode(int mode) {
        [Purchasely setThemeMode:(PLYThemeMode)mode];
    }

	@interface PresentationViewDelegate : NSObject

	@property(nonatomic, copy) void (^presentationResult)(enum PLYProductViewControllerResult result, PLYPlan * _Nullable plan);

	@end

	@implementation PresentationViewDelegate {} @end

	PresentationViewDelegate* _presentationViewDelegate;

	void _purchaselyFetchPresentation(const char* presentationId, const char* contentId,
									  PurchaselyPresentationCallbackDelegate successCallback, void* successCallbackPtr,
									  PurchaselyStringCallbackDelegate errorCallback, void* errorCallbackPtr) {
		NSString* presentationIdStr = [PLYUtils createNSStringFrom:presentationId];
		NSString* contentIdStr = [PLYUtils createNSStringFrom:contentId];

		auto fetchCompletion = ^(PLYPresentation * _Nullable presentation, NSError * _Nullable error) {
			if (presentation != nil) {
				successCallback(successCallbackPtr, [PLYUtils presentationToJson:presentation], (void *) CFBridgingRetain(presentation));
			} else {
				errorCallback(errorCallbackPtr, [PLYUtils createCStringFrom:[error localizedDescription]]);
			}
		};

		auto completionFunction = ^(enum PLYProductViewControllerResult result, PLYPlan * _Nullable plan) {
			if (_presentationViewDelegate == nil)
				return;

			_presentationViewDelegate.presentationResult(result, plan);
		};

		if ([contentIdStr length] == 0) {
			[Purchasely fetchPresentationWith:presentationIdStr fetchCompletion:fetchCompletion completion:completionFunction];
		} else {
			[Purchasely fetchPresentationWith:presentationIdStr contentId:contentIdStr fetchCompletion:fetchCompletion completion:completionFunction];
		}
	}

	void _purchaselyFetchPresentationForPlacement(const char* placementId, const char* contentId,
												  PurchaselyPresentationCallbackDelegate successCallback, void* successCallbackPtr,
												  PurchaselyStringCallbackDelegate errorCallback, void* errorCallbackPtr) {
		NSString* placementIdStr = [PLYUtils createNSStringFrom:placementId];
		NSString* contentIdStr = [PLYUtils createNSStringFrom:contentId];

		auto fetchCompletion = ^(PLYPresentation * _Nullable presentation, NSError * _Nullable error) {
			if (presentation != nil) {
				successCallback(successCallbackPtr, [PLYUtils presentationToJson:presentation], (void *) CFBridgingRetain(presentation));
			} else {
				errorCallback(errorCallbackPtr, [PLYUtils createCStringFrom:[error localizedDescription]]);
			}
		};

		auto completionFunction = ^(enum PLYProductViewControllerResult result, PLYPlan * _Nullable plan) {
			if (_presentationViewDelegate == nil)
				return;

			_presentationViewDelegate.presentationResult(result, plan);
		};

		if ([contentIdStr length] == 0) {
			[Purchasely fetchPresentationFor:placementIdStr fetchCompletion:fetchCompletion completion:completionFunction];
		} else {
			[Purchasely fetchPresentationFor:placementIdStr contentId:contentIdStr fetchCompletion:fetchCompletion completion:completionFunction];
		}
	}

	void _purchaselyShowContentForPresentationObject(void* presentationPtr, PurchaselyVoidCallbackDelegate closeCallback, void* closeCallbackPtr, 	PurchaselyPresentationResultCallbackDelegate presentationResultCallback, void* presentationResultCallbackPtr) {

		_presentationViewDelegate = [PresentationViewDelegate new];
		_presentationViewDelegate.presentationResult = ^(enum PLYProductViewControllerResult result, PLYPlan * _Nullable plan) {
			presentationResultCallback(presentationResultCallbackPtr, [PLYUtils parseProductViewResult:result], [PLYUtils planAsJson:plan]);
		};

		PLYPresentation* presentation = (__bridge PLYPresentation *) (presentationPtr);

		if (presentation != nil) {
			showNavigationControllerForView(presentation.controller);
		} else {
			NSLog(@"Purchasely Presentation is not valid. Will not show.");
		}
	}

	void _purchaselyClientPresentationOpened(void* presentationPtr) {
		PLYPresentation* presentation = (__bridge PLYPresentation *) (presentationPtr);

		if (presentation != nil) {
			[Purchasely clientPresentationOpenedWith:presentation];
		}
	}

	void _purchaselyClientPresentationClosed(void* presentationPtr) {
		PLYPresentation* presentation = (__bridge PLYPresentation *) (presentationPtr);

		if (presentation != nil) {
			[Purchasely clientPresentationClosedWith:presentation];
		}
	}
}
