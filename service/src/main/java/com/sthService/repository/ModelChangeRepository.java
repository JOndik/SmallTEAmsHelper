package com.sthService.repository;

import com.sthService.dataContract.ModelChange;
//import org.springframework.data.jpa.repository.JpaRepository;
import org.springframework.data.mongodb.repository.MongoRepository;
import org.springframework.stereotype.Repository;

@Repository
public interface ModelChangeRepository extends MongoRepository<ModelChange, String> {
}
