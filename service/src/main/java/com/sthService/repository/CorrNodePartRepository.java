package com.sthService.repository;

import com.sthService.dataContract.CorrespondenceNodePart;
import org.springframework.data.mongodb.repository.MongoRepository;
import org.springframework.stereotype.Repository;

@Repository
public interface CorrNodePartRepository extends MongoRepository<CorrespondenceNodePart, String>{

    CorrespondenceNodePart findByUserNameAndElementGUIDAndModelGUIDAndSmallTeamID(String username, String elementGUID, String modelGUID, String smallTeamId);

    CorrespondenceNodePart findById(String id);
}
