package com.sthService.repository;

import com.sthService.dataContract.ModelChange;
import org.springframework.data.domain.Page;
import org.springframework.data.domain.Pageable;
import org.springframework.data.mongodb.repository.MongoRepository;
import org.springframework.stereotype.Repository;

import java.util.List;

/**
 * repository of class ModelChange
 */
@Repository
public interface ModelChangeRepository extends MongoRepository<ModelChange, String> {

    List<ModelChange> findByUserNameAndTimestamp(String username, String timestamp);

    ModelChange findById(String id);

    List<ModelChange> findByTimestamp(String timestamp);

    List<ModelChange> findByUserName(String userName);

    List<ModelChange> findByUserName(String userName, Pageable pageable);
}
