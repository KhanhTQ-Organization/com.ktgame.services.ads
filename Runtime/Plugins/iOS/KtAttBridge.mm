#import <Foundation/Foundation.h>
#import <AppTrackingTransparency/AppTrackingTransparency.h>

typedef void (*KtAttCallback)(int status);

extern "C"
{
    int _KtAttGetStatus()
    {
        if (@available(iOS 14, *))
        {
            return (int)[ATTrackingManager trackingAuthorizationStatus];
        }

        return 3;
    }

    void _KtAttRequest(KtAttCallback callback)
    {
        if (@available(iOS 14, *))
        {
            [ATTrackingManager requestTrackingAuthorizationWithCompletionHandler:^(ATTrackingManagerAuthorizationStatus status)
            {
                dispatch_async(dispatch_get_main_queue(), ^
                {
                    if (callback != NULL)
                    {
                        callback((int)status);
                    }
                });
            }];
            return;
        }

        if (callback != NULL)
        {
            callback(3);
        }
    }
}
