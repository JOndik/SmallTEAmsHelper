package com.sthService.service;

import com.sthService.dataContract.Version;
import com.sthService.repository.UpdateRepository;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Value;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import javax.annotation.PostConstruct;
import javax.inject.Inject;
import javax.validation.Valid;

@Service
@Transactional
public class UpdateService {

    @Inject
    private UpdateRepository updateRepository;

    @Value("${addin.currentVersion}")
    private Double currentVersion;

    private final Logger log = LoggerFactory.getLogger(ModelChangeService.class);

    public Double findVersion() {
        try {
            Version version = updateRepository.findAll().get(0);
            return version.getNumber();
        } catch (NullPointerException e) {
            log.error("Critical - version entry does not exist! ", e.toString());
        }

        return 0.0;
    }

    @PostConstruct
    public void updateVersionEntry() {
        Version version = new Version();
        version.setNumber(currentVersion);

        updateRepository.deleteAll();
        updateRepository.save(version);

        log.info("version was updated");
    }
}
