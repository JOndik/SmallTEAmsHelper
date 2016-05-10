package com.sthService.repository;

import com.sthService.dataContract.User;
import org.springframework.data.mongodb.repository.MongoRepository;
import org.springframework.stereotype.Repository;

/**
 * repository of class User
 */
@Repository
public interface AuthorizationRepository extends MongoRepository<User, String> {

    User findByName(String name);

    User findByToken(String token);

    User findById(String id);

}

