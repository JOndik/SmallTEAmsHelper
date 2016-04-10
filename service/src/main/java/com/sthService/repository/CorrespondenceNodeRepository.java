package com.sthService.repository;

import com.sthService.dataContract.CorrespondenceNode;
import org.springframework.data.mongodb.repository.MongoRepository;
import org.springframework.data.mongodb.repository.Query;
import org.springframework.stereotype.Repository;

@Repository
public interface CorrespondenceNodeRepository extends MongoRepository<CorrespondenceNode, String> {

    CorrespondenceNode findById(String id);

    @Query(value = "{ 'correspondenceNodePartIDs' : ?0 }")
    CorrespondenceNode findByNestedCorrespondenceNodePartId(String id);

    /*@Query(value = "{ 'corrNodePartList.userName' : ?1, 'corrNodePartList.elementGUID' : ?2 }")
    CorrespondenceNode findBySmallTeamIdAndNestedCorrNodePartUserNameAndElementGUID(String smallTeamId, String userName, String elementGUID);           //funguje*/
}
