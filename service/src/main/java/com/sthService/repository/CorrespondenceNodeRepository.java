package com.sthService.repository;

import com.sthService.dataContract.CorrespondenceNode;
import org.springframework.data.mongodb.repository.MongoRepository;
import org.springframework.data.mongodb.repository.Query;
import org.springframework.stereotype.Repository;

@Repository
public interface CorrespondenceNodeRepository extends MongoRepository<CorrespondenceNode, String> {

    CorrespondenceNode findById(String id);

    @Query(value = "{ 'corrNodePartList.userName' : ?0 }")
    CorrespondenceNode findByNestedCorrNodePartUserName(String userName);           //funguje
}
