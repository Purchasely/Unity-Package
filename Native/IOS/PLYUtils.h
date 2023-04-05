#import <Foundation/Foundation.h>
#import <Purchasely/Purchasely-Swift.h>

@interface PLYUtils : NSObject
+ (char*) cStringCopy:(const char*) string;

+ (char*) createCStringFrom:(NSString*) string;

+ (NSString*) createNSStringFrom:(const char*) cstring;

+ (NSString*) serializeDictionary:(NSDictionary*) dictionary;

+ (NSString*) serializeArray:(NSArray*) array;

+ (NSDictionary*) planAsDictionary:(PLYPlan*) plan;

+ (NSDictionary*) productAsDictionary:(PLYProduct*) product;

+ (NSDictionary*) subscriptionAsDictionary:(PLYSubscription*) subscription;

+ (PLYRunningMode) parseRunningMode:(int) mode;

+ (LogLevel) parseLogLevel:(int) level;

+ (char*) planAsJson:(PLYPlan*) plan;

+ (char*) productAsJson:(PLYProduct*) product;

+ (char*) productsAsJson:(NSArray<PLYProduct*>*) products;

+ (char*) susbscriptionsAsJson:(NSArray<PLYSubscription*>*) subscriptions;

+ (int) parseProductViewResult:(PLYProductViewControllerResult) result;

+ (NSDictionary<NSString*, NSObject*>*) resultDictionaryForActionInterceptor:(PLYPresentationAction) action parameters: (PLYPresentationActionParameters* _Nullable) params presentationInfos: (PLYPresentationInfo* _Nullable) infos;

+ (char*) actionToJson:(PLYPresentationAction) action parameters: (PLYPresentationActionParameters* _Nullable) params presentationInfos: (PLYPresentationInfo* _Nullable) infos;

+ (char*) presentationToJson:(PLYPresentation*) presentation;
@end
