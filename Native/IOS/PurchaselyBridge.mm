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
	
	void _purchaselyStart(const char* apiKey, const char* userId, bool readyToPurchase, int logLevel, int runningMode,
						  PurchaselyStartCallbackDelegate startCallback, void* startCallbackPtr,
						  PurchaselyStringCallbackDelegate eventCallback, void* eventCallbackPtr) {
		_eventDelegate = [PurchaselyEventDelegate new];
		_eventDelegate.eventCallback = ^(NSDictionary<NSString*, id>* properties) {
			eventCallback(eventCallbackPtr, [PLYUtils createCStringFrom:[PLYUtils serializeDictionary:properties]]);
		};
	
		[Purchasely startWithAPIKey:[PLYUtils createNSStringFrom:apiKey]
						  appUserId:[PLYUtils createNSStringFrom:userId]
						runningMode:[PLYUtils parseRunningMode:runningMode]
					  eventDelegate:_eventDelegate
						 uiDelegate:nil
		  paywallActionsInterceptor:nil
						   logLevel:[PLYUtils parseLogLevel:logLevel]
						initialized:^(BOOL success, NSError * _Nullable error) {
			NSString* errorString = error == nil ? @"" : [error localizedDescription];
			startCallback(startCallbackPtr, success, [PLYUtils createCStringFrom:errorString]);
		}];
	}
	
	void _purchaselyUserLogin(const char* userId, PurchaselyBoolCallbackDelegate onUserLogin, void* onUserLoginPtr) {
		[Purchasely userLoginWith:[PLYUtils createNSStringFrom:userId] shouldRefresh:^(BOOL shouldRefresh) {
			onUserLogin(onUserLoginPtr, shouldRefresh);
		}];
	}
	
	void _purchaselySetIsReadyToPurchase(bool ready) {
		[Purchasely isReadyToPurchase:ready];
	}
	
	void _purchaselySetDefaultPresentationResultHandler(PurchaselyPresentationResultCallbackDelegate presentationResultCallback, void* 	presentationResultCallbackPtr) {
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
	
	void _purchaselyShowContentForPresentation(const char* presentationId, const char* contentId, PurchaselyBoolCallbackDelegate loadCallback, void* 	loadCallbackPtr, PurchaselyVoidCallbackDelegate closeCallback, void* closeCallbackPtr, PurchaselyPresentationResultCallbackDelegate 	presentationResultCallback, void* presentationResultCallbackPtr) {
	
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
	
	void _purchaselyShowContentForPlacement(const char* placementId, const char* contentId, PurchaselyBoolCallbackDelegate loadCallback, void* 	loadCallbackPtr, PurchaselyVoidCallbackDelegate closeCallback, void* closeCallbackPtr, PurchaselyPresentationResultCallbackDelegate 	presentationResultCallback, void* presentationResultCallbackPtr) {
	
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
	
	void _purchaselyShowContentForPlan(const char* planId, const char* presentationId, const char* contentId, PurchaselyBoolCallbackDelegate 	loadCallback, void* loadCallbackPtr, PurchaselyVoidCallbackDelegate closeCallback, void* closeCallbackPtr, 	PurchaselyPresentationResultCallbackDelegate presentationResultCallback, void* presentationResultCallbackPtr) {
	
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
	
	void _purchaselyShowContentForProduct(const char* productId, const char* presentationId, const char* contentId, PurchaselyBoolCallbackDelegate 	loadCallback, void* loadCallbackPtr, PurchaselyVoidCallbackDelegate closeCallback, void* closeCallbackPtr, 	PurchaselyPresentationResultCallbackDelegate presentationResultCallback, void* presentationResultCallbackPtr) {
	
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
		ctrl.navigationItem.leftBarButtonItem = [[UIBarButtonItem alloc] initWithBarButtonSystemItem: UIBarButtonSystemItemDone target:navCtrl 	action:@selector(close)];
	
		[Purchasely showController:navCtrl type: PLYUIControllerTypeSubscriptionList];
	}
	
	void _purchaselyPurchase(const char* planId, PurchaselyStringCallbackDelegate successCallback, void* successCallbackPtr, 	PurchaselyStringCallbackDelegate errorCallback, void* errorCallbackPtr) {
		NSString *planIdStr = [PLYUtils createNSStringFrom:planId];
	
		[Purchasely planWith:planIdStr success:^(PLYPlan * _Nonnull plan) {
			[Purchasely purchaseWithPlan:plan success:^{
				successCallback(successCallbackPtr, [PLYUtils planAsJson:plan]);
			} failure:^(NSError * _Nonnull error) {
				errorCallback(errorCallbackPtr, [PLYUtils createCStringFrom:error.localizedDescription]);
			}];
		} failure:^(NSError * _Nullable error) {
			errorCallback(errorCallbackPtr, [PLYUtils createCStringFrom:error.localizedDescription]);
		}];
	}
	
	void _purchaselyRestoreAllProducts(bool isSilent, PurchaselyStringCallbackDelegate successCallback, void* successCallbackPtr, 	PurchaselyStringCallbackDelegate errorCallback, void* errorCallbackPtr) {
	
		auto successAction = ^{
			successCallback(successCallbackPtr, [PLYUtils createCStringFrom:@"{}"]);
		};
	
		auto errorAction = ^(NSError * _Nonnull error) {
			errorCallback(errorCallbackPtr, [PLYUtils createCStringFrom:error.localizedDescription]);
		};
	
		if (isSilent) {
			[Purchasely silentRestoreAllProductsWithSuccess:successAction failure:errorAction];
		} else {
			[Purchasely restoreAllProductsWithSuccess:successAction failure:errorAction];
		}
	}
	
	void _purchaselyGetAllProducts(PurchaselyStringCallbackDelegate successCallback, void* successCallbackPtr, PurchaselyStringCallbackDelegate 	errorCallback, void* errorCallbackPtr) {
		[Purchasely allProductsWithSuccess:^(NSArray<PLYProduct*>* _Nonnull products) {
			successCallback(successCallbackPtr, [PLYUtils productsAsJson:products]);
		} failure:^(NSError * _Nullable error) {
			errorCallback(errorCallbackPtr, [PLYUtils createCStringFrom:error.localizedDescription]);
		}];
	}
	
	void _purchaselyGetProduct(const char* productId, PurchaselyStringCallbackDelegate successCallback, void* successCallbackPtr, 	PurchaselyStringCallbackDelegate errorCallback, void* errorCallbackPtr) {
	
		[Purchasely productWith:[PLYUtils createNSStringFrom:productId] success:^(PLYProduct * _Nonnull product) {
			successCallback(successCallbackPtr, [PLYUtils productAsJson:product]);
		} failure:^(NSError * _Nullable error) {
			errorCallback(errorCallbackPtr, [PLYUtils createCStringFrom:error.localizedDescription]);
		}];
	}
	
	void _purchaselyGetPlan(const char* planId, PurchaselyStringCallbackDelegate successCallback, void* successCallbackPtr, 	PurchaselyStringCallbackDelegate errorCallback, void* errorCallbackPtr) {
	
		[Purchasely planWith:[PLYUtils createNSStringFrom:planId] success:^(PLYPlan * _Nonnull plan) {
			successCallback(successCallbackPtr, [PLYUtils planAsJson:plan]);
		} failure:^(NSError * _Nullable error) {
			errorCallback(errorCallbackPtr, [PLYUtils createCStringFrom:error.localizedDescription]);
		}];
	}
	
	void _purchaselyGetUserSubscriptions(PurchaselyStringCallbackDelegate successCallback, void* successCallbackPtr, PurchaselyStringCallbackDelegate 	errorCallback, void* errorCallbackPtr) {
		[Purchasely userSubscriptionsWithSuccess:^(NSArray<PLYSubscription*>* _Nullable subscriptions) {
			successCallback(successCallbackPtr, [PLYUtils susbscriptionsAsJson:subscriptions]);
		} failure:^(NSError * _Nonnull error) {
			errorCallback(errorCallbackPtr, [PLYUtils createCStringFrom:error.localizedDescription]);
		}];
	}
	
	bool _purchaselyHandleUrl(const char* urlString) {
		NSURL* url = [NSURL URLWithString:[PLYUtils createNSStringFrom:urlString]];
		return [Purchasely handleWithDeeplink:url];
	}
	
	void _purchaselySetLanguage(const char* language) {
		NSLocale *locale = [NSLocale localeWithLocaleIdentifier:[PLYUtils createNSStringFrom:language]];
		[Purchasely setLanguageFrom:locale];
	}

	void closePaywall() {
		if (presentedPresentationViewController != nil) {
			dispatch_async(dispatch_get_main_queue(), ^{
				[presentedPresentationViewController dismissViewControllerAnimated:true completion:^{
					presentedPresentationViewController = nil;
				}];
			});
		}
	}
	
	void _purchaselySetPaywallActionInterceptor(PurchaselyStringCallbackDelegate actionCallback, void* actionCallbackPtr) {
		interceptorDelegate = [PurchaselyInterceptorDelegate new];
		interceptorDelegate.actionCallback = ^(char* data) {
			actionCallback(actionCallbackPtr, data);
		};
		
		interceptorFunction = ^(enum PLYPresentationAction action, PLYPresentationActionParameters * _Nullable parameters, 	PLYPresentationInfo * _Nullable infos, void (^ _Nonnull actionHandler)(BOOL)) {
			
			onProcessActionHandler = actionHandler;
			closePaywall();
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
}
