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

    public void saveChange(String username, ModelChange newChange) {
        newChange.setUserName(username);
        modelChangeRepository.save(newChange);
    }

    public List<ModelChange> fetchAllChanges() {
        return modelChangeRepository.findAll();
    }

    public List<ModelChange> findModelData(String timestamp){
        return modelChangeRepository.findByTimestamp(timestamp);
    }

    public ModelChange getChangeById(String id){
        return modelChangeRepository.findById(id);
    }

    public List<ModelChange> getChangesByUsernameAndTimestamp(String userName, String timestamp){
        return modelChangeRepository.findByUserNameAndTimestamp(userName, timestamp);
    }

    public void updateModelChange(ModelChange modelChange){
        modelChangeRepository.save(modelChange);
    }

    public void deleteModelChange(ModelChange modelChange){
        modelChangeRepository.delete(modelChange);
    }
}
