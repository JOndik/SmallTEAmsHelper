package com.sthService.service;

import com.sthService.dataContract.*;
import com.sthService.repository.*;
import com.sun.mail.imap.protocol.Item;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import javax.inject.Inject;
import java.util.ArrayList;
import java.util.List;

@Service
@Transactional
public class SynchronizationService {

    @Inject
    private AuthorizationService authorizationService;

    @Inject
    private SmallTeamRepository smallTeamRepository;

    @Inject
    private ModelChangeRepository modelChangeRepository;

    @Inject
    private CorrespondenceModelRepository correspondenceModelRepository;

    @Inject
    private CorrespondenceNodeRepository correspondenceNodeRepository;

    private final Logger log = LoggerFactory.getLogger(SynchronizationService.class);

    public String checkSynchronizationInfo(String token){
        User actualUser = authorizationService.getUser(token);
        SmallTeam smallTeam = smallTeamRepository.findByNestedUserId(actualUser.getId());
        if(smallTeam == null){
            log.info("noJoin");
            return "noJoin";
        } else {
            log.info(String.valueOf(actualUser.isAllModelData()));
            if (actualUser.isAllModelData()) {                                  //ak uz poslal cely model
                if (smallTeam.isSynchronizationAllowed()) {                     // ak uz prebieha sync
                    log.info("sync");
                    return "true";
                } else {                                                        //ak este neprebieha sync
                    log.info("nosync");
                    return "noSync";
                }
            } else {                                                            //ak este neposlal model
                log.info("nodata");
                return "false";
            }
        }
    }

    public void checkOtherTeamMembers(User actualUser){
        SmallTeam smallTeam = smallTeamRepository.findByNestedUserId(actualUser.getId());
        List<String> teamMembersIds = smallTeam.getTeamMembersId();
        for(String id : teamMembersIds){
            User user = authorizationService.getUserById(id);
            if (!id.equals(actualUser.getId())) {
                if (user.isAllModelData()) {
                    log.info("spojenie dat aktualneho usera s datami vyhladaneho " + user.getName());
                    addDataToCorrespondenceModel(smallTeam, actualUser, user);
                } else {
                    log.info("kolega " + user.getName() + " este neposlal data modelu");
                }
            }
        }
    }

    public void addDataToCorrespondenceModel(SmallTeam smallTeam, User firstUser, User secondUser){
        String corrModelID = smallTeam.getCorrespondenceModelID();
        List<ModelChange> firstUserData = modelChangeRepository.findByUserNameAndTimestamp(firstUser.getName(), "0");
        List<ModelChange> secondUserData = modelChangeRepository.findByUserNameAndTimestamp(secondUser.getName(), "0");

        log.info(String.valueOf(firstUserData.size()) + " " + String.valueOf(secondUserData.size()));

        if (corrModelID == null){                                                               //neexistuje CM
            CorrespondenceModel correspondenceModel = new CorrespondenceModel();
            correspondenceModel.setCorrespondenceNodes(new ArrayList<>());

            CorrespondenceNode corrNode = new CorrespondenceNode();
            corrNode.setCorrNodePartList(new ArrayList<>());
            corrNode.getCorrNodePartList().add(new CorrNodePart(firstUser.getName(), firstUser.getModelGUID()));
            corrNode.getCorrNodePartList().add(new CorrNodePart(secondUser.getName(), secondUser.getModelGUID()));
            correspondenceNodeRepository.save(corrNode);

            correspondenceModel.getCorrespondenceNodes().add(corrNode);
            correspondenceModelRepository.save(correspondenceModel);

            List<CorrespondenceNode> correspondenceNodes = correspondenceModel.getCorrespondenceNodes();

            for (ModelChange dataOfFirst : firstUserData){
                log.info(((ItemCreation)dataOfFirst).getName());

                for (ModelChange dataOfSecond : secondUserData){
                    log.info(((ItemCreation)dataOfFirst).getName() + " " + ((ItemCreation)dataOfSecond).getName());

                    if (dataOfFirst.getElementType() == dataOfSecond.getElementType()){

                        if (dataOfFirst.getElementType() == 800 && ((ItemCreation)dataOfFirst).getName().equals(((ItemCreation)dataOfSecond).getName())){
                            log.info(((ItemCreation) dataOfFirst).getPackageGUID() + " " + ((ItemCreation) dataOfSecond).getPackageGUID());

                        }
                    }
                }
            }

            CorrespondenceNode corr = correspondenceNodeRepository.findByNestedCorrNodePartUserName(firstUser.getName());
            if (corr != null){
                log.info(corr.getId());

            }
            log.info(correspondenceModel.getId());
            CorrespondenceNode corr2 = correspondenceModelRepository.findByIdAndNestedCorrNodePartUserName(correspondenceModel.getId(), firstUser.getName());
            if (corr2 != null){
                log.info(corr2.getId());
                /*for(CorrNodePart cnp : corr2.getCorrNodePartList()){
                    log.info(cnp.getElementGUID());
                }*/

            }

            /*for (ModelChange dataOfFirst : firstUserData) {
                log.info(((ItemCreation)dataOfFirst).getName());
            }*/
            //correspondenceModelRepository.save(correspondenceModel);
        } else {
            log.info("existuje corr model");
        }
    }
}
