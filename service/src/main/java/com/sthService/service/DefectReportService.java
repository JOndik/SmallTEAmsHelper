package com.sthService.service;

import com.sthService.dataContract.DefectReport;
import com.sthService.repository.DefectReportRepository;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import javax.inject.Inject;

@Service
@Transactional
public class DefectReportService {

    @Inject
    private DefectReportRepository defectReportRepository;

    public void saveReport(String username, DefectReport newReport) {
        newReport.setUserName(username);
        DefectReport oldReport = defectReportRepository
                .findByRuleGUIDAndUserNameAndModelGUID(newReport.getRuleGUID(), username, newReport.getModelGUID());

        if (oldReport != null) {
            newReport.setId(oldReport.getId());
        }

        defectReportRepository.save(newReport);
    }
}
