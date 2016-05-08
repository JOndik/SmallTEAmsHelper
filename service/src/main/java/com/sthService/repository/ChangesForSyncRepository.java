package com.sthService.repository;

import com.sthService.dataContract.ChangesForSynchronization;
import org.springframework.data.mongodb.repository.MongoRepository;
import org.springframework.stereotype.Repository;

import java.util.List;

@Repository
public interface ChangesForSyncRepository extends MongoRepository<ChangesForSynchronization, String>{

    ChangesForSynchronization findByUserNameAndSmallTeamId(String userName, String smallTeamId);

    List<ChangesForSynchronization> findBySmallTeamId(String smallTeamId);
}
