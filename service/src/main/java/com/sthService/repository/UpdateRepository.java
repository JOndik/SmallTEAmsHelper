package com.sthService.repository;

import com.sthService.dataContract.Version;
import org.springframework.data.mongodb.repository.MongoRepository;
import org.springframework.stereotype.Repository;

/**
 * repository of class Version
 */
@Repository
public interface UpdateRepository extends MongoRepository<Version, String> {

}
