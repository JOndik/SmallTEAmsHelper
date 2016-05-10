package com.sthService.service;

import com.sthService.dataContract.ChangesForSynchronization;
import com.sthService.repository.ChangesForSyncRepository;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import javax.inject.Inject;
import java.util.ArrayList;
import java.util.List;

@Service
@Transactional
public class ChangesForSynchronizationService {

    @Inject
    private ChangesForSyncRepository changesForSyncRepository;

    /**
     * method finds changes for synchronization by parameters
     * @param userName name of user that has this list
     * @param smallTeamId ID of small team where user is member
     * @return changes for synchronization
     */
    public ChangesForSynchronization findChangesForSynchronization(String userName, String smallTeamId){
        return changesForSyncRepository.findByUserNameAndSmallTeamId(userName, smallTeamId);
    }

    /**
     * method creates instance of ChangesForSynchronization class
     * @param userName name of user
     * @param smallTeamId ID of small team
     */
    public void createChangesForSynchronization(String userName, String smallTeamId){
        ChangesForSynchronization changesForSynchronization = new ChangesForSynchronization();
        changesForSynchronization.setChangeIDs(new ArrayList<>());
        changesForSynchronization.setUserName(userName);
        changesForSynchronization.setSmallTeamId(smallTeamId);
        changesForSyncRepository.save(changesForSynchronization);
    }

    /**
     * method updates instance of ChangesForSynchronization
     * @param changesForSynchronization instance of ChangesForSynchronization
     */
    public void updateChangesForSynchronization(ChangesForSynchronization changesForSynchronization){
        changesForSyncRepository.save(changesForSynchronization);
    }

    /**
     * method finds instances of ChangesForSynchronization by ID of small team
     * @param id ID of small team
     * @return list of ChangesForSynchronization
     */
    public List<ChangesForSynchronization> getChangesForSynchronizationBySmallTeamId(String id){
        return changesForSyncRepository.findBySmallTeamId(id);
    }

    /**
     * method removes instance of ChangesForSynchronization
     * @param changesForSynchronization instance of ChangesForSynchronization
     */
    public void deleteChangesForSynchronization(ChangesForSynchronization changesForSynchronization){
        changesForSyncRepository.delete(changesForSynchronization);
    }
}
