package com.sthService.service;

import com.sthService.dataContract.TeamPairRequest;
import com.sthService.repository.PairRequestRepository;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.stereotype.Service;
import org.springframework.transaction.annotation.Transactional;

import javax.inject.Inject;

@Service
@Transactional
public class PairRequestService {

    private final Logger log = LoggerFactory.getLogger(PairRequestService.class);

    @Inject
    PairRequestRepository pairRequestRepository;

    @Inject
    AISMailService aisMailService;

    public void savePairRequest(TeamPairRequest request) {
        log.info("IDEM ULOZIT\n");
        pairRequestRepository.save(request);
        log.info("IDEM POSLAT MAIL\n");
        aisMailService.sendPairRequestEmail(request.getMemberName(), request.getToken());
    }

    public TeamPairRequest getPairRequest(String pairToken) {
        return pairRequestRepository.findByToken(pairToken);
    }

    public void deleteRequest(TeamPairRequest request) {
        pairRequestRepository.delete(request);
    }
}
