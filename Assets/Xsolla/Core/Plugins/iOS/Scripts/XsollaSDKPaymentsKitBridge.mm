#import "XsollaSDKPaymentsKitObjectiveC/XsollaSDKPaymentsKitObjectiveC-Swift.h"
#import "XsollaUtils.h"

#pragma mark - C interface

typedef void(ActionStringCallbackDelegate)(void *actionPtr, const char *data);

extern "C" {
    
    void _performPayment(char* token, bool isSandbox, char* redirectUrl, ActionStringCallbackDelegate errorCallback, void *errorActionPtr) {

        NSString* tokenString = [XsollaUtils createNSStringFrom:token];
        NSString* redirectUrlString = [XsollaUtils createNSStringFrom:redirectUrl];     
        BOOL isSandboxBool = isSandbox;

        [[PaymentsKitObjectiveC shared] performPaymentWithPaymentToken:tokenString presenter:UnityGetGLViewController() isSandbox:isSandboxBool redirectUrl:redirectUrlString completionHandler:^(NSError * _Nullable error)
        {
            if(error != nil) {
                NSLog(@"Error code: %ld", error.code);
                
                if(error.code != NSError.cancelledByUserError) {
                    errorCallback(errorActionPtr, [XsollaUtils createCStringFrom:error.localizedDescription]);
                }               
            }
        }];
    }

    extern UIViewController *UnityGetGLViewController();
}




