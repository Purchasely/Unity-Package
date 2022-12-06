#import <Foundation/Foundation.h>
#import "UIViewController+Purchasely.h"

@implementation UIViewController (Purchasely)

- (void) close {
	[self dismissViewControllerAnimated:YES completion:nil];
}

@end
