package com.sthService.repository;

import com.sthService.dataContract.DefectReport;
import org.springframework.data.mongodb.repository.MongoRepository;
import org.springframework.stereotype.Repository;

@Repository
public interface DefectReportRepository extends MongoRepository<DefectReport, String> {

    DefectReport findByRuleGUIDAndUserNameAndModelGUID(String ruleGUID, String userName, String modelGUID);
}
