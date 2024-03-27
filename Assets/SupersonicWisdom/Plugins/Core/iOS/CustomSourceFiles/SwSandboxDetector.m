// Based on https://stackoverflow.com/a/27398665/4325134

bool _swIsSandbox() {
    return [[[[NSBundle mainBundle] appStoreReceiptURL] lastPathComponent] isEqualToString:@"sandboxReceipt"];
}