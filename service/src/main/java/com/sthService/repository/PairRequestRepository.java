package com.sthService.repository;

import com.sthService.dataContract.TeamPairRequest;
import org.springframework.data.mongodb.repository.MongoRepository;
import org.springframework.stereotype.Repository;

@Repository
public interface PairRequestRepository extends MongoRepository<TeamPairRequest, String> {

    TeamPairRequest findByToken(String token);
}
