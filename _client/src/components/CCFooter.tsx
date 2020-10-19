import React, { useState, useEffect } from 'react'
import { userIsRoot, getUser, appDispatcher } from '../init'
import AppBar from '@material-ui/core/AppBar';
import Toolbar from '@material-ui/core/Toolbar';

// import SlackIcon from "../imgs/slack.svg";
import { IconButton, Tooltip, Typography, Dialog, DialogTitle, DialogContent, Link, Button } from '@material-ui/core';
import BugReportIcon from '@material-ui/icons/BugReport';
import { openCloseState } from '../utils/StateUtils';
import { Grow } from '../utils/renderers';

const SlackIcon = () => {
    return <>
        <svg aria-hidden="true" focusable="false" width="1em" height="1em" className="slack-icon" preserveAspectRatio="xMidYMid meet" viewBox="0 0 24 24"><path d="M6 15a2 2 0 0 1-2 2a2 2 0 0 1-2-2a2 2 0 0 1 2-2h2v2m1 0a2 2 0 0 1 2-2a2 2 0 0 1 2 2v5a2 2 0 0 1-2 2a2 2 0 0 1-2-2v-5m2-8a2 2 0 0 1-2-2a2 2 0 0 1 2-2a2 2 0 0 1 2 2v2H9m0 1a2 2 0 0 1 2 2a2 2 0 0 1-2 2H4a2 2 0 0 1-2-2a2 2 0 0 1 2-2h5m8 2a2 2 0 0 1 2-2a2 2 0 0 1 2 2a2 2 0 0 1-2 2h-2v-2m-1 0a2 2 0 0 1-2 2a2 2 0 0 1-2-2V5a2 2 0 0 1 2-2a2 2 0 0 1 2 2v5m-2 8a2 2 0 0 1 2 2a2 2 0 0 1-2 2a2 2 0 0 1-2-2v-2h2m0-1a2 2 0 0 1-2-2a2 2 0 0 1 2-2h5a2 2 0 0 1 2 2a2 2 0 0 1-2 2h-5z"/></svg>
    </>
}
export const CcFooter = () => {
    const [isOpen, setIsOpen] = useState(false);
    const [openDialog, closeDialog] = openCloseState(setIsOpen);
    const [user, setUser] = useState(getUser());
    const isRoot = userIsRoot();

    useEffect(() => {
        appDispatcher.register(payload => {
            switch (payload.actionType) {
                case "userChanged":
                    setUser({ ...getUser() });
                    break;
            }
        });
    }, []);

    return <AppBar position="fixed"
        component="footer"
        className={`${isRoot ? "is-root" : "is-student"}`}
        style={{ top: "auto", bottom: 0 }} variant="outlined">
        <Toolbar className="container denser" variant="dense">
            <Grow />
            <Tooltip className="with-pointer-events" enterDelay={0} arrow
                title={<>Report a bug <BugReportIcon /></>}>
                <IconButton onClick={openDialog} size="small">
                    <BugReportIcon htmlColor="#ffffff" />
                </IconButton>
            </Tooltip>
        </Toolbar>

        <Dialog open={isOpen} onClose={closeDialog}>
            <DialogTitle>
                <>Report a bug <BugReportIcon /></>
            </DialogTitle>
            <DialogContent dividers>
                <Typography variant="body1"></Typography>
                <Typography variant="caption">
                    <p>Join our <Button target="_blank" variant="text" size="small" color="secondary" endIcon={<SlackIcon />}
                        href="https://join.slack.com/t/cc-ibu4947/shared_invite/zt-i5wkc06f-yyG~d1w20EfmwTUZJ2bevA">
                            Slack channel
                    </Button>
                    and report a bug or suggest some cool ideas.
                    </p>
                    <p>
                        <em>Please, be as specific as possible when describing a problem. If possible include a screenshot.</em>
                    </p>
                </Typography>
                
            </DialogContent>

        </Dialog>
    </AppBar>
}