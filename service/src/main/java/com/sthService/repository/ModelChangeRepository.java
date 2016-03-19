package com.sthService.repository;

import com.sthService.dataContract.ItemCreation;
import com.sthService.dataContract.ModelChange;
import org.springframework.data.mongodb.repository.MongoRepository;
import org.springframework.stereotype.Repository;

import java.util.List;

@Repository
public interface ModelChangeRepository extends MongoRepository<ModelChange, String> {

    List<ModelChange> findByUserNameAndTimestamp(String username, String timestamp);
}
