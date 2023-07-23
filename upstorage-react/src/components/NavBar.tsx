import { NavLink } from "react-router-dom";
import {Container, Menu,Image} from "semantic-ui-react";

const NavBar = () => {
    return (
        <Menu fixed='top' inverted>
            <Container>
                <Menu.Item as='a' header>
                    <Image size='mini' src='/vite.svg' style={{ marginRight: '1.5em' }} />
                    UpStorage
                </Menu.Item>
                <Menu.Item as={NavLink} to="/order">Order</Menu.Item>
                <Menu.Item as={NavLink} to="/log">Log</Menu.Item>
                <Menu.Item as={NavLink} to="/abc">Not Found</Menu.Item>
            </Container>
        </Menu>
    );
}

export default  NavBar;
