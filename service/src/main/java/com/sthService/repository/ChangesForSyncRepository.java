package com.sthService.repository;

import com.sthService.dataContract.ChangesForSynchronization;
import org.springframework.data.mongodb.repository.MongoRepository;
import org.springframework.stereotype.Repository;

@Repository
public interface ChangesForSyncRepository extends MongoRepository<ChangesForSynchronization, String>{

    ChangesForSynchronization findByUserNameAndSmallTeamId(String userName, String smallTeamId);
}
