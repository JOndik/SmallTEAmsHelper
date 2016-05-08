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

    public void createCorrespondenceNode(String firstId, String secondId){
        CorrespondenceNode newCorrNode = new CorrespondenceNode();
        newCorrNode.setCorrespondenceNodePartIDs(new ArrayList<>());
        newCorrNode.getCorrespondenceNodePartIDs().add(firstId);
        newCorrNode.getCorrespondenceNodePartIDs().add(secondId);
        correspondenceNodeRepository.save(newCorrNode);
    }

    public CorrespondenceNode getCorrNodeByCorrNodePartId(String id){
        return correspondenceNodeRepository.findByNestedCorrespondenceNodePartId(id);
    }

    public void updateCorrespondenceNode(CorrespondenceNode correspondenceNode){
        correspondenceNodeRepository.save(correspondenceNode);
    }

    public void deleteCorrespondenceNode(CorrespondenceNode correspondenceNode){
        correspondenceNodeRepository.delete(correspondenceNode);
    }
}
