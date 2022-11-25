#import <Purchasely/Purchasely-Swift.h>

#pragma clang diagnostic push
#pragma ide diagnostic ignored "OCUnusedGlobalDeclarationInspection"

// Callback definitions
typedef void(PurchaselyStartCallbackDelegate)(void *actionPtr, bool success, const char *error);

typedef void(PurchaselyBoolCallbackDelegate)(void *actionPtr, bool refreshRequired);

typedef void(PurchaselyVoidCallbackDelegate)(void *actionPtr);

typedef void(PurchaselyPresentationResultCallbackDelegate)(void *actionPtr, int result, void* plan);

typedef void(PurchaselyEventCallbackDelegate)(void *actionPtr, const char *name, const char *propertiesJson);

// Event Delegate
@interface PurchaselyEventDelegate : NSObject <PLYEventDelegate>

@property(nonatomic, copy) void (^eventCallback)(NSString* name, NSDictionary<NSString*, id>* properties);

@end

@implementation PurchaselyEventDelegate

- (void)eventTriggered:(enum PLYEvent)event properties:(NSDictionary<NSString *, id> * _Nullable)properties {
	if (_eventCallback == nil)
		return;
	
	_eventCallback([NSString fromPLYEvent:event], properties);
}

@end

// Bridge methods
extern "C" {
	// String converters
	char* cStringCopy(const char* string) {
		char *res = (char *) malloc(strlen(string) + 1);
		strcpy(res, string);
		return res;
	}
	
	char* createCStringFrom(NSString* string) {
		if (!string) {
			string = @"";
		}
		
		return cStringCopy([string UTF8String]);
	}

	NSString* createNSStringFrom(const char* cstring) {
		return [NSString stringWithUTF8String:(cstring ?: "")];
	}

	NSString* serializeDictionary(NSDictionary* dictionary) {
		NSError *error;
		NSData *jsonData = [NSJSONSerialization dataWithJSONObject:dictionary
														options:NSJSONWritingOptions()
															error:&error];
		return [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
	}

	// SDK methods
	PLYRunningMode parseRunningMode(int mode) {
		if (mode == 0) {
			return PLYRunningModeObserver;
		}
		if (mode == 1) {
			return PLYRunningModePaywallObserver;
		}
		if (mode == 2) {
			return PLYRunningModePaywallOnly;
		}
		if (mode == 3) {
			return PLYRunningModeTransactionOnly;
		}
		
		return PLYRunningModeFull;
	}
	
	LogLevel parseLogLevel(int level) {
		if (level == 0) {
			return LogLevelDebug;
		}
		if (level == 2) {
			return LogLevelWarn;
		}
		if (level == 3) {
			return LogLevelError;
		}
		
		return LogLevelInfo;
	}

	PurchaselyEventDelegate* _eventDelegate;
	
	void _purchaselyStart(const char* apiKey, const char* userId, bool readyToPurchase, int logLevel, int runningMode,
						  PurchaselyStartCallbackDelegate startCallback, void* startCallbackPtr,
						  PurchaselyEventCallbackDelegate eventCallback, void* eventCallbackPtr) {
		_eventDelegate = [PurchaselyEventDelegate new];
		_eventDelegate.eventCallback = ^(NSString* name, NSDictionary<NSString*, id>* properties) {
			eventCallback(eventCallbackPtr, createCStringFrom(name), createCStringFrom(serializeDictionary(properties)));
		};
		
		[Purchasely startWithAPIKey:createNSStringFrom(apiKey)
						  appUserId:createNSStringFrom(userId)
						runningMode:parseRunningMode(runningMode)
					  eventDelegate:_eventDelegate
						 uiDelegate:nil
		  paywallActionsInterceptor:nil
						   logLevel:parseLogLevel(logLevel)
						initialized:^(BOOL success, NSError * _Nullable error) {
			NSString* errorString = error == nil ? @"" : [error localizedDescription];
			startCallback(startCallbackPtr, success, createCStringFrom(errorString));
		}];
	}

	void _purchaselyUserLogin(const char* userId, PurchaselyBoolCallbackDelegate onUserLogin, void* onUserLoginPtr) {
		[Purchasely userLoginWith:createNSStringFrom(userId) shouldRefresh:^(BOOL shouldRefresh) {
			onUserLogin(onUserLoginPtr, shouldRefresh);
		}];
	}

	void _purchaselySetIsReadyToPurchase(bool ready) {
		[Purchasely isReadyToPurchase:ready];
	}
	
	int parseProductViewResult(PLYProductViewControllerResult result) {
		if (result == PLYProductViewControllerResultPurchased)
			return 0;
		if (result == PLYProductViewControllerResultRestored)
			return 1;
		
		return 2;
	}

	void _purchaselyShowContentForPlacement(const char* placementId, const char* contentId, bool displayCloseButton, PurchaselyBoolCallbackDelegate 	loadCallback, void* loadCallbackPtr, PurchaselyVoidCallbackDelegate closeCallback, void* closeCallbackPtr, 	PurchaselyPresentationResultCallbackDelegate presentationResultCallback, void* presentationResultCallbackPtr) {
		NSString* contentIdString = createNSStringFrom(placementId);
		
		UIViewController * presentationView;
	
		if (contentIdString.length < 1) {
			presentationView = [Purchasely presentationControllerFor:createNSStringFrom(placementId) contentId:contentIdString loaded:^(PLYPresentationViewController * _Nullable constroller, BOOL loaded, NSError * _Nullable error) {
					if (error != nil) {
						NSLog(@"%@", [error localizedDescription]);
					}
					
					loadCallback(loadCallbackPtr, loaded);
				} completion:^(enum PLYProductViewControllerResult result, PLYPlan * _Nullable plan) {
					presentationResultCallback(presentationResultCallbackPtr, parseProductViewResult(result), (void *) CFBridgingRetain(plan));
			}];
		} else {
			presentationView = [Purchasely presentationControllerFor:createNSStringFrom(placementId) loaded:^(PLYPresentationViewController * _Nullable constroller, BOOL loaded, NSError * _Nullable error) {
				if (error != nil) {
					   NSLog(@"%@", [error localizedDescription]);
				   }
				   
				   loadCallback(loadCallbackPtr, loaded);
			   } completion:^(enum PLYProductViewControllerResult result, PLYPlan * _Nullable plan) {
				   presentationResultCallback(presentationResultCallbackPtr, parseProductViewResult(result), (void *) CFBridgingRetain(plan));
			}];
		}
		
		[UnityGetGLViewController() presentViewController:presentationView animated:false completion:nil];
	}
}
