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

    public ChangesForSynchronization findChangesForSynchronization(String userName, String smallTeamId){
        return changesForSyncRepository.findByUserNameAndSmallTeamId(userName, smallTeamId);
    }

    public void createChangesForSynchronization(String userName, String smallTeamId){
        ChangesForSynchronization changesForSynchronization = new ChangesForSynchronization();
        changesForSynchronization.setChangeIDs(new ArrayList<>());
        changesForSynchronization.setUserName(userName);
        changesForSynchronization.setSmallTeamId(smallTeamId);
        changesForSyncRepository.save(changesForSynchronization);
    }

    public void updateChangesForSynchronization(ChangesForSynchronization changesForSynchronization){
        changesForSyncRepository.save(changesForSynchronization);
    }

    public List<ChangesForSynchronization> getChangesForSynchronizationBySmallTeamId(String id){
        return changesForSyncRepository.findBySmallTeamId(id);
    }

    public void deleteChangesForSynchronization(ChangesForSynchronization changesForSynchronization){
        changesForSyncRepository.delete(changesForSynchronization);
    }
}
