import authList from './auth-list';

export default function(name, user) {
    if (!name || !user.ID) return false;

    let pass = true;
    const auth = authList.find(item => item.name === name);
    // 如果有設定權限
    if (auth) {
        const blacklist = auth.blacklist;
        const matchBlockRole = blacklist.roleID.some(item => item === user.RoleID);
        if (matchBlockRole) {
            pass = false;
        }
    } else {
        pass = true;
    }

    return pass;
}
