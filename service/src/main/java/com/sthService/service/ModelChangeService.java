package com.sthService.service;

import com.sthService.dataContract.ModelChange;
import com.sthService.repository.ModelChangeRepository;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import javax.inject.Inject;
import java.util.List;

@Service
@Transactional
public class ModelChangeService {

    @Inject
    private ModelChangeRepository modelChangeRepository;

    private final Logger log = LoggerFactory.getLogger(ModelChangeService.class);

    public void saveChange(String username, ModelChange newChange) {
        //log.info(((PropertyChange)newChange).getPropertyBody());

        newChange.setUserName(username);
        modelChangeRepository.save(newChange);
    }

    public List<ModelChange> fetchAllChanges() {
        return modelChangeRepository.findAll();
    }
}
