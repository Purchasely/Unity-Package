#include "PLYUtils.h"
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
													options:NSJSONWritingOptions()
														error:&error];
	return [[NSString alloc] initWithData:jsonData encoding:NSUTF8StringEncoding];
}

+ (NSDictionary*) planAsDictionary:(PLYPlan*) plan {
	NSMutableDictionary<NSString *, NSObject *> *dict = [NSMutableDictionary new];
	
	[dict setObject:plan.vendorId forKey:@"vendorId"];
	[dict setObject:@(plan.hasIntroductoryPrice) forKey:@"hasIntroductoryPrice"];
	[dict setObject:@([plan type]) forKey:@"type"];
	[dict setObject:@([plan isEligibleForIntroOffer]) forKey:@"isEligibleForIntroOffer"];
	
	if (plan.hasIntroductoryPrice && [[plan introAmount] intValue] == 0) {
		[dict setObject:@(YES) forKey:@"hasFreeTrial"];
		[dict removeObjectForKey:@"hasIntroductoryPrice"];
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

+ (PLYRunningMode) parseRunningMode:(int) mode {
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

@end
