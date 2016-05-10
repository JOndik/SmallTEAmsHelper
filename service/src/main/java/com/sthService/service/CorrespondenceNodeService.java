package com.sthService.service;

import com.sthService.dataContract.CorrespondenceNode;
import com.sthService.repository.CorrespondenceNodeRepository;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import javax.inject.Inject;
import java.util.ArrayList;

@Service
@Transactional
public class CorrespondenceNodeService {

    @Inject
    private CorrespondenceNodeRepository correspondenceNodeRepository;

    /**
     * method creates new correspondence node
     * @param firstId ID of correspondence node part
     * @param secondId ID of correspondence node part
     */
    public void createCorrespondenceNode(String firstId, String secondId){
        CorrespondenceNode newCorrNode = new CorrespondenceNode();
        newCorrNode.setCorrespondenceNodePartIDs(new ArrayList<>());
        newCorrNode.getCorrespondenceNodePartIDs().add(firstId);
        newCorrNode.getCorrespondenceNodePartIDs().add(secondId);
        correspondenceNodeRepository.save(newCorrNode);
    }

    /**
     * method finds correspondence node by ID of correspondence node part
     * @param id ID of correspondence node part
     * @return found correspondence node
     */
    public CorrespondenceNode getCorrNodeByCorrNodePartId(String id){
        return correspondenceNodeRepository.findByNestedCorrespondenceNodePartId(id);
    }

    /**
     * method updates correspondence node
     * @param correspondenceNode correspondence node to be updated
     */
    public void updateCorrespondenceNode(CorrespondenceNode correspondenceNode){
        correspondenceNodeRepository.save(correspondenceNode);
    }

    /**
     * method removes correspondence node
     * @param correspondenceNode correspondence node to be deleted
     */
    public void deleteCorrespondenceNode(CorrespondenceNode correspondenceNode){
        correspondenceNodeRepository.delete(correspondenceNode);
    }
}
