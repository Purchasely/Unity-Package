#import <Foundation/Foundation.h>
#import <Purchasely/Purchasely-Swift.h>

@interface PLYUtils : NSObject
+ (char*) cStringCopy:(const char*) string;

+ (char*) createCStringFrom:(NSString*) string;

+ (NSString*) createNSStringFrom:(const char*) cstring;

+ (NSString*) serializeDictionary:(NSDictionary*) dictionary;

+ (NSDictionary*) planAsDictionary:(PLYPlan*) plan;

+ (PLYRunningMode) parseRunningMode:(int) mode;

+ (LogLevel) parseLogLevel:(int) level;

+ (char*) planAsJson:(PLYPlan*) plan;
@end
