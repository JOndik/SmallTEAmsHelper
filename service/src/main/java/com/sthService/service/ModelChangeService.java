package com.sthService.service;

import com.sthService.dataContract.DTOWrapper;
import com.sthService.dataContract.ModelChange;
import com.sthService.repository.ModelChangeRepository;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import javax.inject.Inject;

@Service
@Transactional
public class ModelChangeService {

    @Inject
    private ModelChangeRepository modelChangeRepository;

    private final Logger log = LoggerFactory.getLogger(ModelChangeService.class);

    public void saveChange(ModelChange newChange) {
        log.warn(newChange.getTimestamp());
        modelChangeRepository.save(newChange);
    }
}
