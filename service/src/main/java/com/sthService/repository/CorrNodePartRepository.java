package com.sthService.repository;

import com.sthService.dataContract.CorrespondenceNodePart;
import org.springframework.data.mongodb.repository.MongoRepository;
import org.springframework.stereotype.Repository;

import java.util.List;
/**
 * repository of class CorrespondenceNodePart
 */
@Repository
public interface CorrNodePartRepository extends MongoRepository<CorrespondenceNodePart, String>{

    CorrespondenceNodePart findByUserNameAndElementGUIDAndModelGUIDAndSmallTeamID(String username, String elementGUID, String modelGUID, String smallTeamId);

    CorrespondenceNodePart findById(String id);

    List<CorrespondenceNodePart> findBySmallTeamID(String smallTeamId);
}
