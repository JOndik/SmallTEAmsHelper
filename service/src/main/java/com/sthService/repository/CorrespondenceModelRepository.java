package com.sthService.repository;

import com.sthService.dataContract.CorrespondenceModel;
import com.sthService.dataContract.CorrespondenceNode;
import org.springframework.data.mongodb.repository.MongoRepository;
import org.springframework.data.mongodb.repository.Query;
import org.springframework.stereotype.Repository;

@Repository
public interface CorrespondenceModelRepository extends MongoRepository<CorrespondenceModel, String> {

    CorrespondenceModel findById(String id);

    @Query(value = "{ 'id' : ?0, 'correspondenceNodes.corrNodePartList.userName' : ?1 }")
    CorrespondenceNode findByIdAndNestedCorrNodePartUserName(String id, String userName);
}
