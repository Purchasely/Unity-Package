#import "PLYUtils.h"
#import <Foundation/Foundation.h>

@implementation PLYUtils

+ (char*) cStringCopy:(const char*) string {
    char *res = (char *) malloc(strlen(string) + 1);
    strcpy(res, string);
    return res;
}

+ (char*) createCStringFrom:(NSString*) string {
    if (!string) {
        string = @"";
    }
    
    return [self cStringCopy:[string UTF8String]];
}

+ (NSString*) createNSStringFrom:(const char*) cstring {
    return [NSString stringWithUTF8String:(cstring ?: "")];
}

+ (NSString*) serializeDictionary:(NSDictionary*) dictionary {
    NSError *error;
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:dictionary
                                                       options:kNilOptions
                                                         error:&error];
    return [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
}

+ (NSString*) serializeArray:(NSArray*) array {
    NSError *error;
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:array
                                                       options:kNilOptions
                                                         error:&error];
    return [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
}

+ (NSDictionary*) planAsDictionary:(PLYPlan*) plan {
    NSMutableDictionary<NSString *, NSObject *> *dict = [NSMutableDictionary new];
    
    [dict setObject:@(plan.hasIntroductoryPrice) forKey:@"hasIntroductoryPrice"];
    [dict setObject:@([plan type]) forKey:@"type"];
    
    if (plan.hasIntroductoryPrice && [[plan introAmount] intValue] == 0) {
        [dict setObject:@(YES) forKey:@"hasFreeTrial"];
        [dict removeObjectForKey:@"hasIntroductoryPrice"];
    }
    
    if (plan.vendorId != nil) {
        [dict setObject:plan.vendorId forKey:@"vendorId"];
    }
    
    if (plan.name != nil) {
        [dict setObject:plan.name forKey:@"name"];
    }
    
    if (plan.appleProductId != nil) {
        [dict setObject:plan.appleProductId forKey:@"productId"];
    }
    
    NSString *price = [plan localizedFullPriceWithLanguage:nil];
    if (price != nil) {
        [dict setObject:price forKey:@"price"];
    }
    
    NSDecimalNumber *amount = [plan amount];
    if (amount != nil) {
        [dict setObject:amount forKey:@"amount"];
    }
    
    NSString *localizedAmount = [plan localizedPriceWithLanguage:nil];
    if (localizedAmount != nil) {
        [dict setObject:localizedAmount forKey:@"localizedAmount"];
    }
    
    NSDecimalNumber *introAmount = [plan introAmount];
    if (introAmount != nil) {
        [dict setObject:introAmount forKey:@"introAmount"];
    }
    
    NSString *currencyCode = [plan currencyCode];
    if (currencyCode != nil) {
        [dict setObject:currencyCode forKey:@"currencyCode"];
    }
    
    NSString *currencySymbol = [plan currencySymbol];
    if (currencySymbol != nil) {
        [dict setObject:currencySymbol forKey:@"currencySymbol"];
    }
    
    NSString *period = [plan localizedPeriodWithLanguage:nil];
    if (period != nil) {
        [dict setObject:period forKey:@"period"];
    }
    
    NSString *introPrice = [plan localizedFullIntroductoryPriceWithLanguage:nil];
    if (introPrice != nil) {
        [dict setObject:introPrice forKey:@"introPrice"];
    }
    
    NSString *introDuration = [plan localizedIntroductoryDurationWithLanguage:nil];
    if (introDuration != nil) {
        [dict setObject:introDuration forKey:@"introDuration"];
    }
    
    NSString *introPeriod = [plan localizedIntroductoryPeriodWithLanguage:nil];
    if (introPeriod != nil) {
        [dict setObject:introPeriod forKey:@"introPeriod"];
    }
    
    return dict;
}

+ (NSDictionary*) productAsDictionary:(PLYProduct*) product {
    NSMutableDictionary<NSString *, NSObject *> *dict = [NSMutableDictionary new];
    
    [dict setObject:product.vendorId forKey:@"vendorId"];
    
    NSMutableArray *plansArray = [NSMutableArray new];
    for (PLYPlan *plan in product.plans) {
        [plansArray addObject:[self planAsDictionary:plan]];
    }
    
    [dict setObject:plansArray forKey:@"plans"];
    
    if (product.name != nil) {
        [dict setObject:product.name forKey:@"name"];
    }
    
    return dict;
}

+ (NSDictionary*) subscriptionAsDictionary:(PLYSubscription*) subscription {
    NSMutableDictionary<NSString*, NSObject*>* dict = [NSMutableDictionary new];
    
    [dict setObject:[self planAsDictionary:subscription.plan] forKey:@"plan"];
    [dict setObject:[self productAsDictionary:subscription.product] forKey:@"product"];
    
    if (subscription.contentId != nil) {
        [dict setObject:subscription.contentId forKey:@"contentId"];
    }
    
    if (subscription.storeCountry != nil) {
        [dict setObject:subscription.storeCountry forKey:@"storeCountry"];
    }
    
    if (subscription.isFamilyShared != nil) {
        [dict setObject:subscription.storeCountry forKey:@"isFamilyShared"];
    }

    if (subscription.isFamilyShared != nil) {
        [dict setObject:subscription.offerIdentifier forKey:@"offerIdentifier"];
    }
    
    [dict setObject:[NSNumber numberWithInteger:subscription.subscriptionSource] forKey:@"subscriptionSource"];
    [dict setObject:[NSNumber numberWithInteger:subscription.status] forKey:@"status"];
    [dict setObject:[NSNumber numberWithInteger:subscription.offerType] forKey:@"offerType"];
    
    NSDateFormatter *dateFormat = [[NSDateFormatter alloc] init];
    [dateFormat setDateFormat:@"yyyy-MM-dd'T'HH:mm:ssZ"];
    
    if (subscription.nextRenewalDate != nil) {
        [dict setObject:[dateFormat stringFromDate:subscription.nextRenewalDate] forKey:@"nextRenewalDate"];
    }
    
    if (subscription.cancelledDate != nil) {
        [dict setObject:[dateFormat stringFromDate:subscription.cancelledDate] forKey:@"cancelledDate"];
    }
    
    if (subscription.purchasedDate != nil) {
        [dict setObject:[dateFormat stringFromDate:subscription.purchasedDate] forKey:@"purchasedDate"];
    }
    
    return dict;
}

+ (PLYRunningMode) parseRunningMode:(int) mode {
    if (mode == 0) {
        return PLYRunningModeObserver;
    }
    if (mode == 1) {
        return PLYRunningModePaywallObserver;
    }
    if (mode == 2) {
        return PLYRunningModeTransactionOnly;
    }
    
    return PLYRunningModeFull;
}

+ (LogLevel) parseLogLevel:(int) level {
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

+ (char*) planAsJson:(PLYPlan*) plan {
    return [self createCStringFrom:[self serializeDictionary:[self planAsDictionary:plan]]];
}

+ (char*) productAsJson:(PLYProduct*) product {
    return [self createCStringFrom:[self serializeDictionary:[self productAsDictionary:product]]];
}

+ (char*) productsAsJson:(NSArray<PLYProduct*>*) products {
    NSMutableArray *productsArray = [NSMutableArray array];
    
    for (PLYProduct *product in products) {
        if (product != nil) {
            [productsArray addObject: [self productAsDictionary:product]];
        }
    }
    
    return [self createCStringFrom:[self serializeArray:productsArray]];
}

+ (char*) susbscriptionsAsJson:(NSArray<PLYSubscription*>*) subscriptions {
    NSMutableArray *subscriptionsArray = [NSMutableArray array];
    
    for (PLYSubscription *subscription in subscriptions) {
        if (subscription != nil) {
            [subscriptionsArray addObject:[self subscriptionAsDictionary:subscription]];
        }
    }
    
    return [self createCStringFrom:[self serializeArray:subscriptionsArray]];
}

+ (int) parseProductViewResult:(PLYProductViewControllerResult) result {
    if (result == PLYProductViewControllerResultPurchased)
        return 0;
    if (result == PLYProductViewControllerResultRestored)
        return 1;
    
    return 2;
}

+ (NSDictionary<NSString *, NSObject *> *) resultDictionaryForActionInterceptor:(PLYPresentationAction) action parameters: (PLYPresentationActionParameters * _Nullable) params presentationInfos: (PLYPresentationInfo * _Nullable) infos {
    NSMutableDictionary<NSString*, NSObject*>* actionInterceptorResult = [NSMutableDictionary new];

    NSString* actionString;

    switch (action) {
        case PLYPresentationActionLogin:
            actionString = @"login";
            break;
        case PLYPresentationActionPurchase:
            actionString = @"purchase";
            break;
        case PLYPresentationActionClose:
            actionString = @"close";
            break;
        case PLYPresentationActionRestore:
            actionString = @"restore";
            break;
        case PLYPresentationActionNavigate:
            actionString = @"navigate";
            break;
        case PLYPresentationActionPromoCode:
            actionString = @"promo_code";
            break;
        case PLYPresentationActionOpenPresentation:
            actionString = @"open_presentation";
            break;
    }

    [actionInterceptorResult setObject:actionString forKey:@"action"];

    if (infos != nil) {
        NSMutableDictionary<NSString*, NSObject*>* infosResult = [NSMutableDictionary new];
        if (infos.contentId != nil) {
            [infosResult setObject:infos.contentId forKey:@"contentId"];
        }
        if (infos.presentationId != nil) {
            [infosResult setObject:infos.presentationId forKey:@"presentationId"];
        }
        if (infos.placementId != nil) {
            [infosResult setObject:infos.placementId forKey:@"placementId"];
        }
        if (infos.abTestId != nil) {
            [infosResult setObject:infos.abTestId forKey:@"abTestId"];
        }
        if (infos.abTestVariantId != nil) {
            [infosResult setObject:infos.abTestVariantId forKey:@"abTestVariantId"];
        }

        [actionInterceptorResult setObject:infosResult forKey:@"info"];
    }
    if (params != nil) {
        NSMutableDictionary<NSString*, NSObject*>* paramsResult = [NSMutableDictionary new];
        if (params.url != nil) {
            [paramsResult setObject:params.url.absoluteString forKey:@"url"];
        }
        if (params.plan != nil) {
            [paramsResult setObject:[self planAsDictionary:params.plan] forKey:@"plan"];
        }
        if (params.title != nil) {
            [paramsResult setObject:params.title forKey:@"title"];
        }
        if (params.presentation != nil) {
            [paramsResult setObject:params.presentation forKey:@"presentation"];
        }
        if (params.promoOffer != nil) {
            NSMutableDictionary<NSString *, NSObject *> *promoOffer = [NSMutableDictionary new];
            [promoOffer setObject:params.promoOffer.vendorId forKey:@"vendorId"];
            [promoOffer setObject:params.promoOffer.storeOfferId forKey:@"storeOfferId"];
            [paramsResult setObject:promoOffer forKey:@"offer"];
        }
        [actionInterceptorResult setObject:paramsResult forKey:@"parameters"];
    }
    return actionInterceptorResult;
}

+ (char*) actionToJson:(PLYPresentationAction) action parameters: (PLYPresentationActionParameters * _Nullable) params presentationInfos: (PLYPresentationInfo * _Nullable) infos {
    return [self createCStringFrom:
            [self serializeDictionary:
             [self resultDictionaryForActionInterceptor:action parameters:params presentationInfos:infos]]];
}

+ (char*) presentationToJson:(PLYPresentation*) presentation {
	NSMutableDictionary<NSString*, NSObject*>* dict = [NSMutableDictionary new];
	
	if (presentation.id != nil)
		[dict setObject:presentation.id forKey:@"id"];
	[dict setObject:presentation.language forKey:@"language"];
	if (presentation.placementId != nil)
		[dict setObject:presentation.id forKey:@"placementId"];
	if (presentation.audienceId != nil)
		[dict setObject:presentation.id forKey:@"audienceId"];
	if (presentation.abTestId != nil)
		[dict setObject:presentation.id forKey:@"abTestId"];
	if (presentation.abTestVariantId != nil)
		[dict setObject:presentation.id forKey:@"abTestVariantId"];
	
	if (presentation.plans != nil) {
        NSMutableArray *plans = [NSMutableArray new];

        for (PLYPresentationPlan *plan in presentation.plans) {
            NSMutableDictionary<NSString *, NSObject *> *newPlan = [NSMutableDictionary new];
            if (plan.planVendorId != nil) { [newPlan setObject:plan.planVendorId forKey:@"planVendorId"]; }
            if (plan.storeProductId != nil) { [newPlan setObject:plan.storeProductId forKey:@"storeProductId"]; }
            if (plan.offerId!= nil) { [newPlan setObject:plan.offerId forKey:@"offerId"]; }
                [plans addObject:newPlan];
        }
        [dict setObject:plans forKey:@"plans"];
    }

    if (presentation.metadata != nil) {
        
        NSDictionary<NSString *,id> *rawMetadata = [presentation.metadata getRawMetadata];
        NSMutableDictionary<NSString *,id> *resultDict = [NSMutableDictionary dictionary];
        
        dispatch_group_t group = dispatch_group_create();
        dispatch_queue_t queue = dispatch_get_global_queue(DISPATCH_QUEUE_PRIORITY_DEFAULT, 0);
        dispatch_semaphore_t semaphore = dispatch_semaphore_create(0);

        for (NSString *key in rawMetadata)  {
            id value = rawMetadata[key];
            
            if ([value isKindOfClass: [NSString class]]) {
                dispatch_group_enter(group); // Enter the dispatch group before making the async call
                [presentation.metadata getStringWith:key completion:^(NSString * _Nullable result) {
                    [resultDict setObject:result forKey:key];
                    dispatch_group_leave(group); // Leave the dispatch group after the async call is completed
                }];
            } else {
                [resultDict setObject:value forKey:key];
            }
        }

        dispatch_group_notify(group, queue, ^{
            // Code to execute after all async calls are completed
            [dict setObject:resultDict forKey:@"metadata"];
            dispatch_semaphore_signal(semaphore);
        });
        
        // Wait until all async calls are completed
        dispatch_semaphore_wait(semaphore, DISPATCH_TIME_FOREVER);
	}
	
	NSString* typeString = @"unknown";

	switch (presentation.type) {
		case PLYPresentationTypeNormal:
			typeString = @"normal";
			break;
		case PLYPresentationTypeFallback:
			typeString = @"fallback";
			break;
		case PLYPresentationTypeDeactivated:
			typeString = @"deactivated";
			break;
		case PLYPresentationTypeClient:
			typeString = @"client";
			break;
	}

	[dict setObject:typeString forKey:@"type"];
	
    char *jsonCString = [self createCStringFrom:[self serializeDictionary:dict]];
	
	NSLog(@"JSON String: %s", jsonCString);
	return jsonCString;
}

+ (char*) signatureToJson:(PLYOfferSignature*) signature {
    NSMutableDictionary<NSString *, NSObject *> *dict = [NSMutableDictionary new];

    [dict setObject:signature.planVendorId forKey:@"planVendorId"];
    [dict setObject:signature.identifier forKey:@"identifier"];
    [dict setObject:signature.signature forKey:@"signature"];
    [dict setObject:signature.keyIdentifier forKey:@"keyIdentifier"];
    
    NSString *nonceString = [signature.nonce UUIDString];
    NSObject *nonce = (NSObject *)nonceString;
    if (nonce != nil) {
        [dict setObject:nonce forKey:@"nonce"];
    }
    
    NSNumber *timestamp = [NSNumber numberWithDouble:signature.timestamp];
    if (timestamp != nil) {
        [dict setObject:timestamp forKey:@"timestamp"];
    }
    
    NSError *error = nil;
    NSData *jsonData = [NSJSONSerialization dataWithJSONObject:dict options:0 error:&error];
            
    if (jsonData) {
        // Convert JSON data to char *
        const char *jsonCString = [[NSString stringWithUTF8String:[jsonData bytes]] UTF8String];
        NSLog(@"JSON String: %s", jsonCString);
    } else {
        NSLog(@"Error converting dictionary to JSON: %@", error.localizedDescription);
    }
    return nil;
}

@end
