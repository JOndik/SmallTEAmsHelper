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

    /**
     * method saves new change and sets its author
     * @param username name of change author
     * @param newChange new change
     */
    public void saveChange(String username, ModelChange newChange) {
        newChange.setUserName(username);
        modelChangeRepository.save(newChange);
    }

    /**
     * method finds all model changes
     * @return list of all model changes
     */
    public List<ModelChange> fetchAllChanges() {
        return modelChangeRepository.findAll();
    }

    /**
     * method finds data about model according to parameter timestamp
     * @param timestamp time when model change was done
     * @return list of found model changes
     */
    public List<ModelChange> findModelData(String timestamp){
        return modelChangeRepository.findByTimestamp(timestamp);
    }

    /**
     * method finds model change by its ID
     * @param id ID of model change
     * @return found model change
     */
    public ModelChange getChangeById(String id){
        return modelChangeRepository.findById(id);
    }

    /**
     * method finds model changes according to parameters
     * @param userName name of change author
     * @param timestamp time when model change was done
     * @return list of found model changes
     */
    public List<ModelChange> getChangesByUsernameAndTimestamp(String userName, String timestamp){
        return modelChangeRepository.findByUserNameAndTimestamp(userName, timestamp);
    }

    /**
     * method updates model change
     * @param modelChange model change to be updated
     */
    public void updateModelChange(ModelChange modelChange){
        modelChangeRepository.save(modelChange);
    }

    /**
     * method removes model change
     * @param modelChange model change to be removed
     */
    public void deleteModelChange(ModelChange modelChange){
        modelChangeRepository.delete(modelChange);
    }
}
