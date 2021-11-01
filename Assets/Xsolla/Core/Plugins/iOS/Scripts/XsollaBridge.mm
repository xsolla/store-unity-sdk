#import <AuthenticationServices/AuthenticationServices.h>
#import "XsollaSDKLoginKitUnity/XsollaSDKLoginKitUnity-Swift.h"

#pragma mark - C interface

extern "C" {

    void _xsolla_TestSwift() {
		OAuth2Params *oauthParams = [[OAuth2Params alloc] initWithClientId:57
															 state:@"xsollatest"
															 scope:@"offline"
															 redirectUri:@"app://xsollalogin"];
		
		JWTGenerationParams *jwtGenerationParams = [[JWTGenerationParams alloc] initWithGrantType:TokenGrantTypeAuthorizationCode
															 clientId:57
															 refreshToken:nil
															 clientSecret:nil
															 redirectUri:@"app://xsollalogin"];
		
		if (@available(iOS 13.4, *)) {
			WebAuthenticationPresentationContextProvider* context = [[WebAuthenticationPresentationContextProvider alloc] initWithPresentationAnchor:UnityGetMainWindow()];
			
			[[LoginKitUnity shared] authBySocialNetwork:@"google" oAuth2Params:oauthParams jwtParams:jwtGenerationParams presentationContextProvider:context completion:^(AccessTokenInfo * _Nullable accesTokenInfo, NSError * _Nullable error){

				if(error != nil) {
					NSLog(@"Error: %@", error);
				}
				else {
					NSLog(@"Received token: %@", accesTokenInfo.accessToken);
				}
				
			}];
		} else {
			NSLog(@"Authentication via social networks with Xsolla is not supported for current iOS version.");
		}
    }
}
