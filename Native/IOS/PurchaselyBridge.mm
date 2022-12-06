#import <Purchasely/Purchasely-Swift.h>
#include "PLYUtils.h"

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

// Bridge methods
extern "C" {
	PurchaselyEventDelegate* _eventDelegate;
	
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
	
	int parseProductViewResult(PLYProductViewControllerResult result) {
		if (result == PLYProductViewControllerResultPurchased)
			return 0;
		if (result == PLYProductViewControllerResultRestored)
			return 1;
		
		return 2;
	}

	void _purchaselyShowContentForPlacement(const char* placementId, const char* contentId, PurchaselyBoolCallbackDelegate loadCallback, void* loadCallbackPtr, PurchaselyVoidCallbackDelegate closeCallback, void* closeCallbackPtr, 	PurchaselyPresentationResultCallbackDelegate presentationResultCallback, void* presentationResultCallbackPtr) {
		NSString* contentIdString = [PLYUtils createNSStringFrom:placementId];
		
		UIViewController * presentationView;
		
		NSString* placementIdStr = [PLYUtils createNSStringFrom:placementId];
	
		if (contentIdString.length < 1) {
			presentationView = [Purchasely presentationControllerFor:placementIdStr contentId:contentIdString loaded:^(PLYPresentationViewController * _Nullable constroller, BOOL loaded, NSError * _Nullable error) {
					if (error != nil) {
						NSLog(@"%@", [error localizedDescription]);
					}
					
					loadCallback(loadCallbackPtr, loaded);
				} completion:^(enum PLYProductViewControllerResult result, PLYPlan * _Nullable plan) {
					presentationResultCallback(presentationResultCallbackPtr, parseProductViewResult(result), [PLYUtils planAsJson:plan]);
			}];
		} else {
			presentationView = [Purchasely presentationControllerFor:placementIdStr loaded:^(PLYPresentationViewController * _Nullable constroller, BOOL loaded, NSError * _Nullable error) {
				if (error != nil) {
					   NSLog(@"%@", [error localizedDescription]);
				   }
				   
				   loadCallback(loadCallbackPtr, loaded);
			   } completion:^(enum PLYProductViewControllerResult result, PLYPlan * _Nullable plan) {
				   presentationResultCallback(presentationResultCallbackPtr, parseProductViewResult(result), [PLYUtils planAsJson:plan]);
			}];
		}
		
		[UnityGetGLViewController() presentViewController:presentationView animated:false completion:nil];
	}
}
