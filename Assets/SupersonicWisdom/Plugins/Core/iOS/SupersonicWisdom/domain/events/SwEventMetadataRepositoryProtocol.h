//
//  SwEventMetadataRepositoryProtocol.h
//  SupersonicWisdom
//
//  Created by Andrey Michailov on 17/11/2020.
//

@protocol SwEventMetadataRepositoryProtocol <NSObject>

@required
- (void)put:(SwEventMetadataDto *)metadata;
- (SwEventMetadataDto *)get;
- (void)update:(SwEventMetadataDto *)metadata;

@end
