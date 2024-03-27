int _swGetNativeWidth() {
    CGRect screenBounds = [[UIScreen mainScreen] nativeBounds];
    CGFloat width = floor(CGRectGetWidth(screenBounds));
    return (int)(width);
}

int _swGetNativeHeight() {
    CGRect screenBounds = [[UIScreen mainScreen] nativeBounds];
    CGFloat height = floor(CGRectGetHeight(screenBounds));
    return (int)(height);
}