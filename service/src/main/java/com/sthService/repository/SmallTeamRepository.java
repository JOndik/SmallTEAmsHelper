package com.sthService.repository;

import com.sthService.dataContract.SmallTeam;
import org.springframework.data.mongodb.repository.MongoRepository;
import org.springframework.data.mongodb.repository.Query;
import org.springframework.stereotype.Repository;

@Repository
public interface SmallTeamRepository extends MongoRepository<SmallTeam, String> {

    @Query(value = "{ 'teamMembersId' : ?0 }")
    SmallTeam findByNestedUserId(String id);
}
